using System.Collections.Generic;

namespace ET.Test
{
    [EnableClass]
    public static class SlotOpsChecks
    {
        public static int Flows()
        {
            OpsCatalog catalog = Catalog();
            OpsPlayerState ops = new OpsPlayerState();
            SlotPlayerState player = new SlotPlayerState();
            player.Add(1, 10);
            player.Add(2, 0);
            player.Add(10001, 1);
            ops.AddProgress(1, 10);
            if (OpsServices.ClaimTask(catalog, ops, player, 100001) != 0 || player.Get(2) != 5000)
            {
                return 1;
            }

            if (OpsServices.ClaimTask(catalog, ops, player, 100001) != OpsServices.Already)
            {
                return 2;
            }

            OpsServices.CreateOrder(catalog, ops, 101, "order-1");
            if (OpsServices.DevPay(catalog, ops, player, "order-1", new DevPaymentPort()) != 0)
            {
                return 3;
            }

            if (OpsServices.VipLevel(catalog, ops.VipExp) != 1 || player.RechargeCents != 3000)
            {
                return 4;
            }

            if (OpsServices.ClaimJubao(catalog, ops, player, 100) != 0 || player.Get(1) != 9)
            {
                return 5;
            }

            if (OpsServices.RollDice(catalog, ops, player, 1, false) != 0 || ops.DiceCell != 2)
            {
                return 6;
            }

            Dictionary<int, int> quota = new Dictionary<int, int>();
            if (OpsServices.ClaimKing(catalog, ops, player, 1, 100, quota) != 0)
            {
                return 7;
            }

            if (OpsServices.ClaimKing(catalog, new OpsPlayerState(), player, 1, 100, quota) != OpsServices.Quota)
            {
                return 8;
            }

            if (OpsServices.RivalDamage(catalog, 1, 1, 10) != 50)
            {
                return 9;
            }

            if (OpsServices.RivalDamage(catalog, 1, 1, 1000) != 80)
            {
                return 10;
            }

            OpsMail mail = OpsServices.SendMail(ops, "hi", new List<SlotItemGrant> { new SlotItemGrant(2, 7) });
            if (OpsServices.ClaimMail(ops, player, mail.MailId) != 0 || OpsServices.ClaimMail(ops, player, mail.MailId) != OpsServices.Already)
            {
                return 11;
            }

            if (OpsServices.Redeem(catalog, ops, player, "DEV") != 0 || OpsServices.Redeem(catalog, ops, player, "DEV") != OpsServices.Already)
            {
                return 12;
            }

            player.Wallet[2] = 0;
            if (OpsServices.ClaimBankruptcy(catalog, ops, player) != 0 || player.Get(2) != 10000)
            {
                return 13;
            }

            OpsServices.AccruePiggy(catalog, ops, 100000);
            if (ops.Piggy != 500 || OpsServices.ClaimPiggy(catalog, ops, player) != 0 || ops.Piggy != 0)
            {
                return 14;
            }

            if (OpsServices.SubmitIdentity(ops, new DevIdentityPort(), "a", "12345") == 0)
            {
                return 15;
            }

            if (OpsServices.SubmitIdentity(ops, new DevIdentityPort(), "a", "123456") != 0 || !ops.RealNameVerified)
            {
                return 16;
            }

            if (OpsServices.BindPhone(ops, new DevPhoneBindPort(), "1380000", "000000") != 0 || new DevChannelCoefficientPort().AdditiveDelta("x") != 0)
            {
                return 17;
            }

            if (!new DevAdProofPort().Verify("dev-ad") || new DevAdProofPort().Verify("no"))
            {
                return 18;
            }

            return 0;
        }

        private static OpsCatalog Catalog()
        {
            OpsCatalog catalog = new OpsCatalog();
            catalog.Tasks.Add(new OpsTaskDef
            {
                TaskId = 100001,
                Condition = 1,
                Target = 10,
                Rewards = new List<SlotItemGrant> { new SlotItemGrant(2, 5000) },
            });
            catalog.Products.Add(new OpsProductDef
            {
                ProductId = 101,
                PriceCents = 3000,
                VipExp = 30,
                CountsTowardJubao = true,
                Rewards = new List<SlotItemGrant> { new SlotItemGrant(2, 600000) },
            });
            catalog.Vip.Add(new OpsVipDef { Level = 0, Exp = 0, PiggyMax = 20000 });
            catalog.Vip.Add(new OpsVipDef { Level = 1, Exp = 30, PiggyMax = 60000 });
            catalog.Jubao.Add(new OpsJubaoDef { Level = 1, Gold = 4000, CostItemId = 1, CostCount = 1, CooldownSeconds = 10, RechargeCents = 0 });
            catalog.Dice.Add(new OpsDiceDef { Cell = 1, Rewards = new List<SlotItemGrant>() });
            catalog.Dice.Add(new OpsDiceDef { Cell = 2, Rewards = new List<SlotItemGrant> { new SlotItemGrant(2, 10) } });
            catalog.King.Add(new OpsKingDef { TierId = 1, TargetDamage = 100, Quota = 1, Rewards = new List<SlotItemGrant> { new SlotItemGrant(2, 1) } });
            catalog.Ranks.Add(new OpsRankDef { Period = 1, Place = 1, RivalStep = 50, StopAtPlayerDamage = 80 });
            catalog.RedeemCodes["DEV"] = new List<SlotItemGrant> { new SlotItemGrant(2, 3) };
            catalog.BankruptcyGold = 10000;
            catalog.BankruptcyPerDay = 2;
            return catalog;
        }
    }
}
