namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_SpinHandler : MessageLocationHandler<SlotPlayer, G2Slot_Spin, Slot2G_Spin>
    {
        [EnableGetComponent(typeof(CoroutineLockComponent))]
        protected override async ETTask Run(SlotPlayer player, G2Slot_Spin request, Slot2G_Spin response)
        {
            SlotWorldComponent world = player.Root().GetComponent<SlotWorldComponent>();
            if (world.Maintenance)
            {
                response.Error = ErrorCode.ERR_SlotMaintenance;
                return;
            }

            long seed = request.Seed != 0 ? request.Seed : player.GetSingleton<TimeInfo>().ServerNow();
            EntityRef<SlotPlayer> playerRef = player;
            using (await player.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.SlotLedger, player.Id))
            {
                player = playerRef;
                world = player.Root().GetComponent<SlotWorldComponent>();
                SlotRoundResult result = SlotSpinService.Spin(world.Catalog, player.Spin, new SlotSpinRequest
                {
                    PlayerId = player.Id,
                    ClientSpinId = request.ClientSpinId,
                    RoomId = request.RoomId,
                    StakeId = request.StakeId,
                    LineCount = request.LineCount,
                    Seed = seed,
                }, world.Ledgers, world.Spins);
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

            await ETTask.CompletedTask;
        }
    }
}
