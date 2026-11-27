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

            SlotWorldComponent slotWorld = root.GetComponent<SlotWorldComponent>();
            OpsStateComponent opsState = player.GetComponent<OpsStateComponent>();
            if (opsState == null)
            {
                opsState = player.AddComponent<OpsStateComponent>();
                SlotOpsPersistence.Load(slotWorld, player, opsState.State, opsWorld.KingQuota);
            }

            long now = player.GetSingleton<TimeInfo>().ServerNow() / 1000;
            int code = request.Action >= 20
                ? Query(slotWorld, opsWorld, player, opsState.State, request, response)
                : Dispatch(opsWorld, opsState.State, player.Spin, request, response, now);
            response.Error = code == OpsServices.Ok ? ErrorCode.ERR_Success : SlotErrorMap.FromOps(code);
            bool mutating = request.Action < 20 || request.Action == 30 || request.Action == 31 || request.Action == 32 || request.Action == 33 || request.Action == 36;
            if (code == OpsServices.Ok && mutating)
            {
                SlotOpsPersistence.Save(slotWorld, player, opsState.State, opsWorld.KingQuota, null);
                SlotPush.Wallet(player);
            }

            SlotProtoMap.FillWallet(response.Wallet, player.Spin);
            await ETTask.CompletedTask;
        }

        private static int Query(SlotWorldComponent slotWorld, OpsWorldComponent opsWorld, SlotPlayer player, OpsPlayerState ops, G2Slot_Ops request, Slot2G_Ops response)
        {
            switch (request.Action)
            {
                case 20:
                    foreach (OpsTaskDef task in opsWorld.Catalog.Tasks)
                    {
                        SlotTaskProto row = SlotTaskProto.Create();
                        row.TaskId = task.TaskId;
                        row.TaskType = task.TaskType;
                        row.Condition = task.Condition;
                        row.Progress = ops.Progress(task.Condition);
                        row.Target = task.Target;
                        row.Claimed = ops.ClaimedTasks.Contains(task.TaskId) ? 1 : 0;
                        response.Tasks.Add(row);
                    }
                    return OpsServices.Ok;
                case 21:
                    response.Value = ops.JubaoLevel;
                    response.Amount = ops.JubaoReadyUnix;
                    foreach (OpsJubaoDef row in opsWorld.Catalog.Jubao)
                    {
                        if (row.Level == ops.JubaoLevel)
                        {
                            response.Amount2 = row.Gold;
                        }
                    }
                    return OpsServices.Ok;
                case 22:
                    response.Value = ops.DiceCell;
                    response.Value2 = ops.DiceLaps;
                    response.Value3 = opsWorld.Catalog.Dice.Count;
                    return OpsServices.Ok;
                case 23:
                    response.Amount = player.Spin.TotalDamage;
                    return OpsServices.Ok;
                case 24:
                    response.Amount = ops.Piggy;
                    return OpsServices.Ok;
                case 25:
                    response.Value = ops.BankruptcyClaims;
                    response.Value2 = opsWorld.Catalog.BankruptcyPerDay;
                    response.Amount = opsWorld.Catalog.BankruptcyGold;
                    return OpsServices.Ok;
                case 26:
                    foreach (OpsProductDef product in opsWorld.Catalog.Products)
                    {
                        SlotProductProto row = SlotProductProto.Create();
                        row.ProductId = product.ProductId;
                        row.Category = product.Category;
                        row.PriceCents = product.PriceCents;
                        row.LimitCount = product.LimitCount;
                        int bought;
                        ops.PurchaseCount.TryGetValue(product.ProductId, out bought);
                        row.Bought = bought;
                        response.Products.Add(row);
                    }
                    return OpsServices.Ok;
                case 27:
                    response.Value = OpsServices.VipLevel(opsWorld.Catalog, ops.VipExp);
                    response.Value2 = ops.VipExp;
                    OpsVipDef vip = OpsServices.VipRow(opsWorld.Catalog, response.Value);
                    response.Value3 = vip != null && vip.CanRename ? 1 : 0;
                    response.Value4 = vip != null && vip.CanIdle ? 1 : 0;
                    return OpsServices.Ok;
                case 28:
                    foreach (OpsOrder order in ops.Orders.Values)
                    {
                        SlotOrderProto row = SlotOrderProto.Create();
                        row.OrderId = order.OrderId;
                        row.ProductId = order.ProductId;
                        row.Paid = order.Paid ? 1 : 0;
                        response.Orders.Add(row);
                    }
                    return OpsServices.Ok;
                case 29:
                    foreach (OpsMail mail in ops.Mail)
                    {
                        if (mail.Deleted)
                        {
                            continue;
                        }
                        SlotMailProto row = SlotMailProto.Create();
                        row.MailId = mail.MailId;
                        row.Title = mail.Title ?? string.Empty;
                        row.Body = mail.Body ?? string.Empty;
                        row.Read = mail.Read ? 1 : 0;
                        row.Claimed = mail.Claimed ? 1 : 0;
                        SlotProtoMap.FillGrants(row.Rewards, mail.Rewards);
                        response.Mails.Add(row);
                    }
                    return OpsServices.Ok;
                case 30:
                    return MarkMail(slotWorld, player, ops, request.Arg3, true, false, false);
                case 31:
                    foreach (OpsMail mail in ops.Mail)
                    {
                        if (!mail.Claimed && !mail.Deleted)
                        {
                            OpsServices.ClaimMail(ops, player.Spin, mail.MailId);
                            SlotOpsPersistence.UpdateMail(slotWorld, player, mail);
                        }
                    }
                    return OpsServices.Ok;
                case 32:
                    return MarkMail(slotWorld, player, ops, request.Arg3, false, false, true);
                case 33:
                    OpsMail created = OpsServices.SendMail(ops, request.Text, new System.Collections.Generic.List<SlotItemGrant>());
                    created.Body = request.Text2 ?? string.Empty;
                    if (request.Arg1 > 0 && request.Arg3 > 0)
                    {
                        created.Rewards.Add(new SlotItemGrant(request.Arg1, request.Arg3));
                    }
                    SlotOpsPersistence.InsertMail(slotWorld, player, created);
                    SlotPush.Mail(player, created.MailId, created.Title);
                    response.Amount = created.MailId;
                    return OpsServices.Ok;
                case 34:
                    if (slotWorld.Database != null)
                    {
                        foreach (ET.Slot.Postgres.SlotRankRow row in slotWorld.Database.ListRank(request.Arg1, 50))
                        {
                            SlotRankProto proto = SlotRankProto.Create();
                            proto.PlayerId = row.PlayerId;
                            proto.Nickname = row.Nickname;
                            proto.Damage = row.Damage;
                            response.Ranks.Add(proto);
                        }
                    }
                    response.Amount = OpsServices.RivalDamage(opsWorld.Catalog, request.Arg1, 1, player.Spin.TotalDamage);
                    return OpsServices.Ok;
                case 35:
                    response.Text = player.Nickname ?? string.Empty;
                    response.Text2 = player.Avatar ?? string.Empty;
                    response.Value = OpsServices.VipLevel(opsWorld.Catalog, ops.VipExp);
                    response.Value2 = ops.VipExp;
                    response.Value3 = ops.RealNameVerified ? 1 : 0;
                    response.Value4 = ops.PhoneBound ? 1 : 0;
                    response.Amount = Unread(ops);
                    response.Amount2 = Claimable(opsWorld, ops);
                    response.Message = slotWorld.Maintenance ? "1" : "0";
                    return OpsServices.Ok;
                case 36:
                    OpsVipDef rename = OpsServices.VipRow(opsWorld.Catalog, OpsServices.VipLevel(opsWorld.Catalog, ops.VipExp));
                    if (rename == null || !rename.CanRename)
                    {
                        return OpsServices.NotReady;
                    }
                    if (string.IsNullOrEmpty(request.Text))
                    {
                        return OpsServices.Missing;
                    }
                    player.Nickname = request.Text;
                    response.Text = player.Nickname;
                    return OpsServices.Ok;
                default:
                    return OpsServices.Missing;
            }
        }

        private static int MarkMail(SlotWorldComponent world, SlotPlayer player, OpsPlayerState ops, long mailId, bool read, bool claim, bool delete)
        {
            foreach (OpsMail mail in ops.Mail)
            {
                if (mail.MailId != mailId)
                {
                    continue;
                }
                if (read)
                {
                    mail.Read = true;
                }
                if (claim)
                {
                    OpsServices.ClaimMail(ops, player.Spin, mailId);
                }
                if (delete)
                {
                    mail.Deleted = true;
                }
                SlotOpsPersistence.UpdateMail(world, player, mail);
                return OpsServices.Ok;
            }
            return OpsServices.Missing;
        }

        private static long Unread(OpsPlayerState ops)
        {
            long count = 0;
            foreach (OpsMail mail in ops.Mail)
            {
                if (!mail.Read && !mail.Deleted)
                {
                    count += 1;
                }
            }
            return count;
        }

        private static long Claimable(OpsWorldComponent world, OpsPlayerState ops)
        {
            long count = 0;
            foreach (OpsTaskDef task in world.Catalog.Tasks)
            {
                if (!ops.ClaimedTasks.Contains(task.TaskId) && ops.Progress(task.Condition) >= task.Target)
                {
                    count += 1;
                }
            }
            return count;
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
