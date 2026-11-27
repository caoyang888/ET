using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public static class OpsServices
    {
        public const int Ok = 0;
        public const int Missing = 1;
        public const int Already = 2;
        public const int NotReady = 3;
        public const int NotEnough = 4;
        public const int Closed = 5;
        public const int Quota = 6;
        public const int Limit = 7;

        public static int VipLevel(OpsCatalog catalog, int exp)
        {
            int level = 0;
            foreach (OpsVipDef row in catalog.Vip)
            {
                if (exp >= row.Exp && row.Level >= level)
                {
                    level = row.Level;
                }
            }

            return level;
        }

        public static OpsVipDef VipRow(OpsCatalog catalog, int level)
        {
            foreach (OpsVipDef row in catalog.Vip)
            {
                if (row.Level == level)
                {
                    return row;
                }
            }

            return null;
        }

        public static int ClaimTask(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, int taskId)
        {
            if (ops.ClaimedTasks.Contains(taskId))
            {
                return Already;
            }

            OpsTaskDef task = null;
            foreach (OpsTaskDef row in catalog.Tasks)
            {
                if (row.TaskId == taskId)
                {
                    task = row;
                    break;
                }
            }

            if (task == null || ops.Progress(task.Condition) < task.Target)
            {
                return NotReady;
            }

            ops.ClaimedTasks.Add(taskId);
            Grant(player, task.Rewards);
            return Ok;
        }

        public static OpsOrder CreateOrder(OpsCatalog catalog, OpsPlayerState ops, int productId, string orderId)
        {
            OpsOrder order = new OpsOrder();
            order.OrderId = orderId;
            order.ProductId = productId;
            ops.Orders[orderId] = order;
            return order;
        }

        public static int DevPay(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, string orderId, IPaymentPort payment)
        {
            OpsOrder order;
            if (!ops.Orders.TryGetValue(orderId, out order))
            {
                return Missing;
            }

            if (order.Granted)
            {
                return Already;
            }

            if (!payment.MarkPaid(order))
            {
                return NotReady;
            }

            OpsProductDef product = FindProduct(catalog, order.ProductId);
            if (product == null)
            {
                return Missing;
            }

            int bought;
            ops.PurchaseCount.TryGetValue(product.ProductId, out bought);
            if (product.LimitCount > 0 && bought >= product.LimitCount)
            {
                return Limit;
            }

            order.Granted = true;
            ops.PurchaseCount[product.ProductId] = bought + 1;
            ops.VipExp += product.VipExp;
            if (product.CountsTowardJubao)
            {
                ops.AddProgress(2, product.PriceCents);
                player.RechargeCents += (int)product.PriceCents;
            }

            Grant(player, product.Rewards);
            ops.AddProgress(3, 1);
            return Ok;
        }

        public static int ClaimJubao(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, long nowUnix)
        {
            if (nowUnix < ops.JubaoReadyUnix)
            {
                return NotReady;
            }

            OpsJubaoDef row = null;
            foreach (OpsJubaoDef item in catalog.Jubao)
            {
                if (item.Level == ops.JubaoLevel)
                {
                    row = item;
                    break;
                }
            }

            if (row == null)
            {
                return Missing;
            }

            if (player.RechargeCents < row.RechargeCents)
            {
                return NotReady;
            }

            if (player.Get(row.CostItemId) < row.CostCount)
            {
                return NotEnough;
            }

            player.Add(row.CostItemId, -row.CostCount);
            player.Add(2, row.Gold);
            ops.JubaoReadyUnix = nowUnix + row.CooldownSeconds;
            int next = ops.JubaoLevel;
            foreach (OpsJubaoDef item in catalog.Jubao)
            {
                if (item.Level > ops.JubaoLevel && player.RechargeCents >= item.RechargeCents && item.Level > next)
                {
                    next = item.Level;
                }
            }

            ops.JubaoLevel = next;
            return Ok;
        }

        public static int RollDice(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, int steps, bool lucky)
        {
            if (catalog.Dice.Count == 0 || steps < 1)
            {
                return Missing;
            }

            int itemId = lucky ? 10002 : 10001;
            if (player.Get(itemId) < 1)
            {
                return NotEnough;
            }

            player.Add(itemId, -1);
            int cells = catalog.Dice.Count;
            int index = ops.DiceCell - 1;
            for (int i = 0; i < steps; ++i)
            {
                index += 1;
                if (index >= cells)
                {
                    index = 0;
                    ops.DiceLaps += 1;
                }
            }

            ops.DiceCell = catalog.Dice[index].Cell;
            Grant(player, catalog.Dice[index].Rewards);
            return Ok;
        }

        public static int ClaimLap(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, int laps)
        {
            if (ops.ClaimedLaps.Contains(laps) || ops.DiceLaps < laps)
            {
                return NotReady;
            }

            foreach (OpsLapDef row in catalog.Laps)
            {
                if (row.Laps != laps)
                {
                    continue;
                }

                ops.ClaimedLaps.Add(laps);
                Grant(player, row.Rewards);
                return Ok;
            }

            return Missing;
        }

        public static int ClaimKing(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, int tierId, long damage, Dictionary<int, int> quota)
        {
            if (ops.ClaimedKing.Contains(tierId))
            {
                return Already;
            }

            foreach (OpsKingDef row in catalog.King)
            {
                if (row.TierId != tierId)
                {
                    continue;
                }

                if (damage < row.TargetDamage)
                {
                    return NotReady;
                }

                int taken;
                quota.TryGetValue(tierId, out taken);
                if (taken >= row.Quota)
                {
                    return Quota;
                }

                quota[tierId] = taken + 1;
                ops.ClaimedKing.Add(tierId);
                Grant(player, row.Rewards);
                return Ok;
            }

            return Missing;
        }

        public static long RivalDamage(OpsCatalog catalog, int period, int place, long playerDamage)
        {
            foreach (OpsRankDef row in catalog.Ranks)
            {
                if (row.Period == period && row.Place == place)
                {
                    if (playerDamage >= row.StopAtPlayerDamage)
                    {
                        return row.StopAtPlayerDamage;
                    }

                    return row.RivalStep;
                }
            }

            return 0;
        }

        public static int ClaimRank(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, int rankId, int place)
        {
            foreach (OpsRankDef row in catalog.Ranks)
            {
                if (row.RankId == rankId && row.Place == place)
                {
                    Grant(player, row.Rewards);
                    return Ok;
                }
            }

            return Missing;
        }

        public static OpsMail SendMail(OpsPlayerState ops, string title, List<SlotItemGrant> rewards)
        {
            OpsMail mail = new OpsMail();
            mail.MailId = ops.NextMailId++;
            mail.Title = title;
            mail.Rewards = rewards ?? new List<SlotItemGrant>();
            ops.Mail.Add(mail);
            return mail;
        }

        public static int ClaimMail(OpsPlayerState ops, SlotPlayerState player, long mailId)
        {
            foreach (OpsMail mail in ops.Mail)
            {
                if (mail.MailId != mailId)
                {
                    continue;
                }

                if (mail.Claimed)
                {
                    return Already;
                }

                mail.Read = true;
                mail.Claimed = true;
                Grant(player, mail.Rewards);
                return Ok;
            }

            return Missing;
        }

        public static int Redeem(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player, string code)
        {
            if (ops.Redeemed.Contains(code))
            {
                return Already;
            }

            List<SlotItemGrant> rewards;
            if (!catalog.RedeemCodes.TryGetValue(code, out rewards))
            {
                return Missing;
            }

            ops.Redeemed.Add(code);
            Grant(player, rewards);
            return Ok;
        }

        public static int AccruePiggy(OpsCatalog catalog, OpsPlayerState ops, long stake)
        {
            int level = VipLevel(catalog, ops.VipExp);
            OpsVipDef row = VipRow(catalog, level);
            long cap = row == null || row.PiggyMax < 0 ? long.MaxValue : row.PiggyMax;
            long add = stake * 5 / 1000;
            if (ops.Piggy + add > cap)
            {
                add = cap - ops.Piggy;
            }

            if (add < 0)
            {
                add = 0;
            }

            ops.Piggy += add;
            return Ok;
        }

        public static int ClaimPiggy(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player)
        {
            if (ops.Piggy <= 0)
            {
                return NotReady;
            }

            player.Add(2, ops.Piggy);
            ops.Piggy = 0;
            return Ok;
        }

        public static int ClaimBankruptcy(OpsCatalog catalog, OpsPlayerState ops, SlotPlayerState player)
        {
            if (player.Get(2) > 0)
            {
                return NotReady;
            }

            if (ops.BankruptcyClaims >= catalog.BankruptcyPerDay)
            {
                return Limit;
            }

            ops.BankruptcyClaims += 1;
            player.Add(2, catalog.BankruptcyGold);
            return Ok;
        }

        public static int SubmitIdentity(OpsPlayerState ops, IIdentityPort identity, string name, string number)
        {
            if (!identity.Verify(name, number))
            {
                return NotReady;
            }

            ops.RealNameVerified = true;
            ops.AddProgress(5, 1);
            return Ok;
        }

        public static int BindPhone(OpsPlayerState ops, IPhoneBindPort phone, string number, string code)
        {
            if (!phone.Bind(number, code))
            {
                return NotReady;
            }

            ops.PhoneBound = true;
            ops.AddProgress(6, 1);
            return Ok;
        }

        public static bool ActivityOpen(OpsCatalog catalog, int activityId, long nowUnix)
        {
            foreach (OpsActivityDef row in catalog.Activities)
            {
                if (row.ActivityId != activityId)
                {
                    continue;
                }

                if (row.BeginUnix > 0 && nowUnix < row.BeginUnix)
                {
                    return false;
                }

                if (row.EndUnix > 0 && nowUnix >= row.EndUnix)
                {
                    return false;
                }

                return true;
            }

            return true;
        }

        private static OpsProductDef FindProduct(OpsCatalog catalog, int productId)
        {
            foreach (OpsProductDef row in catalog.Products)
            {
                if (row.ProductId == productId)
                {
                    return row;
                }
            }

            return null;
        }

        private static void Grant(SlotPlayerState player, List<SlotItemGrant> rewards)
        {
            for (int i = 0; i < rewards.Count; ++i)
            {
                player.Add(rewards[i].ItemId, rewards[i].Count);
            }
        }
    }
}
