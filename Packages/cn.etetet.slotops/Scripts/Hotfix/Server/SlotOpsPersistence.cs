using System.Collections.Generic;
using System.Text.Json;
using ET.Slot.Postgres;

namespace ET.Server
{
    public static class SlotOpsPersistence
    {
        public static void Load(SlotWorldComponent world, SlotPlayer player, OpsPlayerState ops, Dictionary<int, int> quota)
        {
            if (world.Database == null)
            {
                return;
            }

            SlotSnapshot snapshot = world.Database.Load(player.Account);
            if (snapshot == null)
            {
                return;
            }

            if (snapshot.Account != null)
            {
                ops.VipExp = snapshot.Account.VipExp;
                ops.RealNameVerified = snapshot.Account.RealNameVerified;
                ops.PhoneBound = snapshot.Account.PhoneBound;
            }

            ApplyOps(ops, snapshot.Ops);
            ops.Orders.Clear();
            foreach (SlotOrderRow row in snapshot.Orders)
            {
                ops.Orders[row.OrderId] = new OpsOrder
                {
                    OrderId = row.OrderId,
                    ProductId = row.ProductId,
                    Paid = row.Status == "paid",
                    Granted = row.Status == "paid",
                };
            }

            ops.Mail.Clear();
            foreach (SlotMailRow row in snapshot.Mails)
            {
                OpsMail mail = new OpsMail
                {
                    MailId = row.MailId,
                    Title = row.Title,
                    Body = row.Body,
                    Read = row.IsRead,
                    Claimed = row.IsClaimed,
                    Deleted = row.IsDeleted,
                    ExpireUnix = row.ExpireUnix,
                };
                if (!string.IsNullOrEmpty(row.AttachmentJson))
                {
                    List<SlotItemGrant> rewards = JsonSerializer.Deserialize<List<SlotItemGrant>>(row.AttachmentJson, SlotJson.Options());
                    if (rewards != null)
                    {
                        mail.Rewards = rewards;
                    }
                }

                ops.Mail.Add(mail);
            }

            if (ops.NextMailId <= 1)
            {
                long max = 1;
                foreach (OpsMail mail in ops.Mail)
                {
                    if (mail.MailId >= max)
                    {
                        max = mail.MailId + 1;
                    }
                }

                ops.NextMailId = max;
            }

            foreach (KeyValuePair<int, int> pair in world.Database.LoadKingQuota())
            {
                quota[pair.Key] = pair.Value;
            }
        }

        public static void Save(SlotWorldComponent world, SlotPlayer player, OpsPlayerState ops, Dictionary<int, int> quota, SlotRoundResult result)
        {
            if (world.Database == null)
            {
                return;
            }

            SlotPersistence.Save(world, player, result, quota);
            SlotSnapshot snapshot = world.Database.Load(player.Account);
            if (snapshot == null || snapshot.Account == null)
            {
                return;
            }

            snapshot.Account.VipExp = ops.VipExp;
            snapshot.Account.RealNameVerified = ops.RealNameVerified;
            snapshot.Account.PhoneBound = ops.PhoneBound;
            snapshot.Ops = ToOps(ops);
            snapshot.Orders.Clear();
            foreach (OpsOrder order in ops.Orders.Values)
            {
                snapshot.Orders.Add(new SlotOrderRow
                {
                    OrderId = order.OrderId,
                    PlayerId = player.Id,
                    ProductId = order.ProductId,
                    Amount = order.Amount,
                    Status = order.Granted ? "paid" : "created",
                    Receipt = order.Paid ? "dev" : string.Empty,
                    CreatedUnix = 0,
                });
            }

            snapshot.Ranks.Clear();
            snapshot.Ranks.Add(new SlotRankRow
            {
                PlayerId = player.Id,
                Nickname = player.Nickname,
                Period = 1,
                Damage = player.Spin.TotalDamage,
            });
            world.Database.Save(snapshot, null, new List<SlotLedgerRow>(), quota, world.Maintenance);
        }

        public static long InsertMail(SlotWorldComponent world, SlotPlayer player, OpsMail mail)
        {
            if (world.Database == null)
            {
                mail.MailId = 0;
                return 0;
            }

            long id = world.Database.InsertMail(player.Account, new SlotMailRow
            {
                PlayerId = player.Id,
                Title = mail.Title,
                Body = mail.Body,
                AttachmentJson = JsonSerializer.Serialize(mail.Rewards, SlotJson.Options()),
                IsRead = mail.Read,
                IsClaimed = mail.Claimed,
                ExpireUnix = mail.ExpireUnix,
            });
            mail.MailId = id;
            return id;
        }

        public static void UpdateMail(SlotWorldComponent world, SlotPlayer player, OpsMail mail)
        {
            if (world.Database == null)
            {
                return;
            }

            world.Database.UpdateMail(player.Account, new SlotMailRow
            {
                MailId = mail.MailId,
                AttachmentJson = JsonSerializer.Serialize(mail.Rewards, SlotJson.Options()),
                IsRead = mail.Read,
                IsClaimed = mail.Claimed,
                IsDeleted = mail.Deleted,
            });
        }

        private static void ApplyOps(OpsPlayerState ops, SlotOpsRow row)
        {
            if (row == null)
            {
                return;
            }

            ops.JubaoLevel = row.JubaoLevel;
            ops.JubaoReadyUnix = row.JubaoReadyUnix;
            ops.DiceCell = row.DiceCell <= 0 ? 1 : row.DiceCell;
            ops.DiceLaps = row.DiceLaps;
            ops.Piggy = row.Piggy;
            ops.BankruptcyClaims = row.BankruptcyClaims;
            Dictionary<string, long> tasks = JsonSerializer.Deserialize<Dictionary<string, long>>(string.IsNullOrEmpty(row.TaskJson) ? "{}" : row.TaskJson, SlotJson.Options());
            if (tasks != null)
            {
                foreach (KeyValuePair<string, long> pair in tasks)
                {
                    ops.TaskProgress[int.Parse(pair.Key)] = pair.Value;
                }
            }

            ReadSet(row.ClaimJson, ops.ClaimedTasks);
            ReadSet(row.KingJson, ops.ClaimedKing);
            ReadSet(row.LapJson, ops.ClaimedLaps);
            List<string> codes = JsonSerializer.Deserialize<List<string>>(string.IsNullOrEmpty(row.RedeemJson) ? "[]" : row.RedeemJson, SlotJson.Options());
            if (codes != null)
            {
                foreach (string code in codes)
                {
                    ops.Redeemed.Add(code);
                }
            }
        }

        private static SlotOpsRow ToOps(OpsPlayerState ops)
        {
            Dictionary<string, long> tasks = new Dictionary<string, long>();
            foreach (KeyValuePair<int, long> pair in ops.TaskProgress)
            {
                tasks[pair.Key.ToString()] = pair.Value;
            }

            return new SlotOpsRow
            {
                JubaoLevel = ops.JubaoLevel,
                JubaoReadyUnix = ops.JubaoReadyUnix,
                DiceCell = ops.DiceCell,
                DiceLaps = ops.DiceLaps,
                Piggy = ops.Piggy,
                BankruptcyClaims = ops.BankruptcyClaims,
                TaskJson = JsonSerializer.Serialize(tasks, SlotJson.Options()),
                ClaimJson = JsonSerializer.Serialize(ops.ClaimedTasks, SlotJson.Options()),
                KingJson = JsonSerializer.Serialize(ops.ClaimedKing, SlotJson.Options()),
                LapJson = JsonSerializer.Serialize(ops.ClaimedLaps, SlotJson.Options()),
                RedeemJson = JsonSerializer.Serialize(ops.Redeemed, SlotJson.Options()),
            };
        }

        private static void ReadSet(string json, HashSet<int> target)
        {
            List<int> values = JsonSerializer.Deserialize<List<int>>(string.IsNullOrEmpty(json) ? "[]" : json, SlotJson.Options());
            if (values == null)
            {
                return;
            }

            foreach (int value in values)
            {
                target.Add(value);
            }
        }
    }
}
