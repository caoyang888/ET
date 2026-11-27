namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_OpsHandler : MessageLocationHandler<SlotPlayer, G2Slot_Ops, Slot2G_Ops>
    {
        protected override async ETTask Run(SlotPlayer player, G2Slot_Ops request, Slot2G_Ops response)
        {
            Scene root = player.Root();
            OpsWorldComponent opsWorld = root.GetComponent<OpsWorldComponent>();
            if (opsWorld == null)
            {
                opsWorld = root.AddComponent<OpsWorldComponent>();
                opsWorld.Catalog = OpsGeneratedCatalog.Load(root.Fiber());
            }

            OpsStateComponent opsState = player.GetComponent<OpsStateComponent>();
            if (opsState == null)
            {
                opsState = player.AddComponent<OpsStateComponent>();
                OpsServices.SendMail(opsState.State, "welcome", new System.Collections.Generic.List<SlotItemGrant>());
            }

            long now = player.GetSingleton<TimeInfo>().ServerNow() / 1000;
            int code = Dispatch(opsWorld, opsState.State, player.Spin, request, response, now);
            response.Error = SlotErrorMap.FromOps(code);
            SlotProtoMap.FillWallet(response.Wallet, player.Spin);
            await ETTask.CompletedTask;
        }

        private static int Dispatch(OpsWorldComponent world, OpsPlayerState ops, SlotPlayerState spin, G2Slot_Ops request, Slot2G_Ops response, long now)
        {
            switch (request.Action)
            {
                case 1:
                    return OpsServices.ClaimTask(world.Catalog, ops, spin, request.Arg1);
                case 2:
                    OpsServices.CreateOrder(world.Catalog, ops, request.Arg1, request.Text);
                    response.Message = request.Text;
                    return OpsServices.Ok;
                case 3:
                    int paid = OpsServices.DevPay(world.Catalog, ops, spin, request.Text, world.Payment);
                    response.Value = OpsServices.VipLevel(world.Catalog, ops.VipExp);
                    return paid;
                case 4:
                    return OpsServices.ClaimJubao(world.Catalog, ops, spin, now);
                case 5:
                    int rolled = OpsServices.RollDice(world.Catalog, ops, spin, request.Arg1, request.Arg2 != 0);
                    response.Value = ops.DiceCell;
                    response.Amount = ops.DiceLaps;
                    return rolled;
                case 6:
                    return OpsServices.ClaimLap(world.Catalog, ops, spin, request.Arg1);
                case 7:
                    return OpsServices.ClaimKing(world.Catalog, ops, spin, request.Arg1, spin.TotalDamage, world.KingQuota);
                case 8:
                    return OpsServices.ClaimMail(ops, spin, request.Arg3);
                case 9:
                    return OpsServices.Redeem(world.Catalog, ops, spin, request.Text);
                case 10:
                    OpsServices.AccruePiggy(world.Catalog, ops, request.Arg3);
                    return OpsServices.ClaimPiggy(world.Catalog, ops, spin);
                case 11:
                    return OpsServices.ClaimBankruptcy(world.Catalog, ops, spin);
                case 12:
                    return OpsServices.SubmitIdentity(ops, world.Identity, request.Text, request.Text2);
                case 13:
                    return OpsServices.BindPhone(ops, world.Phone, request.Text, request.Text2);
                case 15:
                    response.Amount = OpsServices.RivalDamage(world.Catalog, request.Arg1, request.Arg2, spin.TotalDamage);
                    return OpsServices.Ok;
                default:
                    return OpsServices.Missing;
            }
        }
    }
}
