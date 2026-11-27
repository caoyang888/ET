namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_SpinHandler : MessageLocationHandler<SlotPlayer, G2Slot_Spin, Slot2G_Spin>
    {
        [EnableGetComponent(typeof(CoroutineLockComponent))]
        protected override async ETTask Run(SlotPlayer player, G2Slot_Spin request, Slot2G_Spin response)
        {
            EntityRef<SlotPlayer> playerRef = player;
            SlotWorldComponent world = player.Root().GetComponent<SlotWorldComponent>();
            if (world.Maintenance)
            {
                response.Error = ErrorCode.ERR_SlotMaintenance;
                return;
            }

            using (await player.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.SlotLedger, player.Id))
            {
                player = playerRef;
                world = player.Root().GetComponent<SlotWorldComponent>();
                SlotRoundResult cached = SlotPersistence.FindSpin(world, player.Account, request.ClientSpinId);
                if (cached != null)
                {
                    Fill(response, player, cached);
                    return;
                }

                long seed = request.Seed != 0 ? request.Seed : player.GetSingleton<TimeInfo>().ServerNow();
                SlotRoundResult result = SlotSpinService.Spin(world.Catalog, player.Spin, new SlotSpinRequest
                {
                    PlayerId = player.Id,
                    ClientSpinId = request.ClientSpinId,
                    RoomId = request.RoomId,
                    StakeId = request.StakeId,
                    LineCount = request.LineCount,
                    Seed = seed,
                }, world.Ledgers, world.Spins);
                if (result.Error == SlotFail.Ok)
                {
                    ApplyOps(player, world, result);
                    long maxHp = 0;
                    SlotRoomDef room;
                    if (world.Catalog.Rooms.TryGetValue(result.RoomId, out room))
                    {
                        SlotStageDef stage;
                        SlotProgressState progress;
                        if (player.Spin.Progress.TryGetValue(room.ProgressGroup, out progress) && world.Catalog.Stages.TryGetValue(progress.StageId, out stage))
                        {
                            maxHp = stage.HpMax;
                        }
                    }

                    SlotPush.Wallet(player);
                    SlotPush.Progress(player, result.RoomId, result.StageId, result.BossHp, maxHp, result.FreeSpinsLeft);
                }

                Fill(response, player, result);
            }

            await ETTask.CompletedTask;
        }

        private static void ApplyOps(SlotPlayer player, SlotWorldComponent world, SlotRoundResult result)
        {
            OpsWorldComponent opsWorld = player.Root().GetComponent<OpsWorldComponent>();
            if (opsWorld == null)
            {
                opsWorld = player.Root().AddComponent<OpsWorldComponent>();
                opsWorld.Catalog = OpsGeneratedCatalog.Load(player.Fiber());
            }

            OpsStateComponent opsState = player.GetComponent<OpsStateComponent>();
            if (opsState == null)
            {
                opsState = player.AddComponent<OpsStateComponent>();
                SlotOpsPersistence.Load(world, player, opsState.State, opsWorld.KingQuota);
            }

            if (result.Damage > 0)
            {
                opsState.State.AddProgress(1, result.Damage);
                opsState.State.AddProgress(8, result.Damage);
            }

            if (result.StakeGold > 0)
            {
                OpsServices.AccruePiggy(opsWorld.Catalog, opsState.State, result.StakeGold);
            }

            SlotOpsPersistence.Save(world, player, opsState.State, opsWorld.KingQuota, result);
        }

        private static void Fill(Slot2G_Spin response, SlotPlayer player, SlotRoundResult result)
        {
            response.Error = SlotErrorMap.FromSpin(result.Error);
            response.Payout = result.Payout;
            response.Damage = result.Damage;
            response.StageId = result.StageId;
            response.BossHp = result.BossHp;
            response.FreeSpins = result.FreeSpinsLeft;
            response.Seed = result.Seed;
            SlotProtoMap.FillSteps(response.Steps, result);
            SlotProtoMap.FillWallet(response.Wallet, player.Spin);
            SlotProtoMap.FillGrants(response.Grants, result.Grants);
        }
    }
}
