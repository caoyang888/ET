using System.Collections.Generic;

namespace ET.Server
{
    public static class OpsGeneratedCatalogFill
    {
        public static void Fill(Fiber fiber, OpsCatalog catalog)
        {
            foreach (OpsVipConfig row in fiber.GetSingleton<OpsVipConfigCategory>().GetAll().Values)
            {
                catalog.Vip.Add(new OpsVipDef
                {
                    Level = row.Id,
                    Exp = row.Exp,
                    PiggyMax = row.PiggyMax,
                    CanIdle = row.CanIdle != 0,
                    CanRename = row.CanRename != 0,
                    CanService = row.CanService != 0,
                });
            }

            Dictionary<int, OpsProductDef> products = new Dictionary<int, OpsProductDef>();
            foreach (OpsProductConfig row in fiber.GetSingleton<OpsProductConfigCategory>().GetAll().Values)
            {
                OpsProductDef product = new OpsProductDef
                {
                    ProductId = row.Id,
                    Category = row.Category,
                    PriceCents = row.PriceCents,
                    LimitCount = row.LimitCount,
                    VipExp = row.VipExpFlag == 0 ? 0 : (int)(row.PriceCents / 100),
                    CountsTowardJubao = row.JubaoFlag != 0,
                };
                products[row.Id] = product;
                catalog.Products.Add(product);
            }

            Dictionary<int, OpsTaskDef> tasks = new Dictionary<int, OpsTaskDef>();
            foreach (OpsTaskConfig row in fiber.GetSingleton<OpsTaskConfigCategory>().GetAll().Values)
            {
                OpsTaskDef task = new OpsTaskDef
                {
                    TaskId = row.Id,
                    TaskType = row.TaskType,
                    Condition = row.Condition,
                    Param = row.Param,
                    Target = row.Target,
                };
                tasks[row.Id] = task;
                catalog.Tasks.Add(task);
            }

            foreach (OpsRewardConfig row in fiber.GetSingleton<OpsRewardConfigCategory>().GetAll().Values)
            {
                SlotItemGrant grant = new SlotItemGrant(row.ItemId, row.Count);
                if (row.Owner == "product" && products.ContainsKey(row.OwnerId))
                {
                    products[row.OwnerId].Rewards.Add(grant);
                }

                if (row.Owner == "task" && tasks.ContainsKey(row.OwnerId))
                {
                    tasks[row.OwnerId].Rewards.Add(grant);
                }
            }

            foreach (OpsJubaoConfig row in fiber.GetSingleton<OpsJubaoConfigCategory>().GetAll().Values)
            {
                catalog.Jubao.Add(new OpsJubaoDef
                {
                    Level = row.Id,
                    Gold = row.Gold,
                    RechargeCents = row.RechargeCents,
                    CostItemId = row.CostItemId,
                    CostCount = row.CostCount,
                    CooldownSeconds = row.CooldownSeconds,
                });
            }

            foreach (OpsDiceConfig row in fiber.GetSingleton<OpsDiceConfigCategory>().GetAll().Values)
            {
                OpsDiceDef cell = new OpsDiceDef
                {
                    Cell = row.Cell,
                    Weight = row.Weight,
                    LuckyWeight = row.LuckyWeight,
                };
                if (row.Count > 0)
                {
                    cell.Rewards.Add(new SlotItemGrant(row.ItemId, row.Count));
                }

                catalog.Dice.Add(cell);
            }

            foreach (OpsLapConfig row in fiber.GetSingleton<OpsLapConfigCategory>().GetAll().Values)
            {
                catalog.Laps.Add(new OpsLapDef
                {
                    Laps = row.Id,
                    Rewards = new List<SlotItemGrant> { new SlotItemGrant(row.ItemId, row.Count) },
                });
            }

            Dictionary<int, OpsRankDef> ranks = new Dictionary<int, OpsRankDef>();
            foreach (OpsRankConfig row in fiber.GetSingleton<OpsRankConfigCategory>().GetAll().Values)
            {
                OpsRankDef rank = new OpsRankDef
                {
                    RankId = row.Id,
                    Period = row.Period,
                    Place = row.Place,
                };
                if (row.Count > 0)
                {
                    rank.Rewards.Add(new SlotItemGrant(row.ItemId, row.Count));
                }

                ranks[row.Id] = rank;
                catalog.Ranks.Add(rank);
            }

            foreach (OpsRivalConfig row in fiber.GetSingleton<OpsRivalConfigCategory>().GetAll().Values)
            {
                OpsRankDef rank;
                if (ranks.TryGetValue(row.Id, out rank))
                {
                    rank.RivalStep = row.Step;
                    rank.StopAtPlayerDamage = row.StopAtPlayerDamage;
                }
            }

            foreach (OpsKingConfig row in fiber.GetSingleton<OpsKingConfigCategory>().GetAll().Values)
            {
                catalog.King.Add(new OpsKingDef
                {
                    TierId = row.Id,
                    TargetDamage = row.TargetDamage,
                    Quota = row.Quota,
                    Rewards = new List<SlotItemGrant> { new SlotItemGrant(row.ItemId, row.Count) },
                });
            }

            foreach (OpsActivityConfig row in fiber.GetSingleton<OpsActivityConfigCategory>().GetAll().Values)
            {
                catalog.Activities.Add(new OpsActivityDef
                {
                    ActivityId = row.Id,
                    BeginUnix = row.BeginUnix,
                    EndUnix = row.EndUnix,
                });
            }

            foreach (OpsRedeemConfig row in fiber.GetSingleton<OpsRedeemConfigCategory>().GetAll().Values)
            {
                catalog.RedeemCodes[row.Code] = new List<SlotItemGrant> { new SlotItemGrant(row.ItemId, row.Count) };
            }

            foreach (OpsReliefConfig row in fiber.GetSingleton<OpsReliefConfigCategory>().GetAll().Values)
            {
                catalog.BankruptcyPerDay = row.PerDay;
                catalog.BankruptcyGold = row.Gold;
            }
        }
    }
}
