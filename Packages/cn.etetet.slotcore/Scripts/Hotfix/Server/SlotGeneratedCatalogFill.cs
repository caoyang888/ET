using System.Collections.Generic;

namespace ET.Server
{
    public static class SlotGeneratedCatalogFill
    {
        public static void Fill(Fiber fiber, SlotCatalog catalog)
        {
            SlotRuleConfig rule = fiber.GetSingleton<SlotRuleConfigCategory>().Get(1);
            catalog.Rule.GoldItemId = rule.GoldItemId;
            catalog.Rule.JadeItemId = rule.JadeItemId;
            catalog.Rule.NewPlayerJade = rule.NewPlayerJade;
            catalog.Rule.LimitDamageRate = rule.LimitDamageRate;
            catalog.Rule.DamagePrice = rule.DamagePrice;
            catalog.Rule.PiggyMilli = rule.PiggyMilli;
            catalog.Pity.MissThreshold = rule.MissThreshold;
            catalog.Pity.CooldownSpins = rule.CooldownSpins;

            foreach (SlotFaceConfig face in fiber.GetSingleton<SlotFaceConfigCategory>().GetAll().Values)
            {
                catalog.Rule.MultiplierFaces.Add(new SlotMultiplierFace { Face = face.Face, Weight = face.Weight });
            }

            foreach (SlotSymbolConfig row in fiber.GetSingleton<SlotSymbolConfigCategory>().GetAll().Values)
            {
                catalog.Symbols[row.Id] = new SlotSymbolDef
                {
                    SymbolId = row.Id,
                    MachineId = row.MachineId,
                    Kind = (SlotSymbolKind)row.Kind,
                    Weight = row.Weight,
                };
            }

            foreach (SlotPayConfig row in fiber.GetSingleton<SlotPayConfigCategory>().GetAll().Values)
            {
                SlotSymbolDef symbol;
                if (catalog.Symbols.TryGetValue(row.SymbolId, out symbol))
                {
                    symbol.PayMilli[row.Count] = row.Milli;
                }
            }

            foreach (SlotMachineConfig row in fiber.GetSingleton<SlotMachineConfigCategory>().GetAll().Values)
            {
                SlotMachineDef machine = new SlotMachineDef
                {
                    MachineId = row.Id,
                    Evaluator = (SlotEvaluatorKind)row.Evaluator,
                    Rows = row.Rows,
                    Columns = row.Columns,
                    MaxCascade = row.MaxCascade,
                    ProgressGroup = row.ProgressGroup,
                };
                if (row.ClearGold > 0)
                {
                    machine.ClearReward.Add(new SlotItemGrant(rule.GoldItemId, row.ClearGold));
                }

                catalog.Machines[row.Id] = machine;
            }

            foreach (SlotSymbolDef symbol in catalog.Symbols.Values)
            {
                SlotMachineDef machine;
                if (catalog.Machines.TryGetValue(symbol.MachineId, out machine))
                {
                    machine.SymbolIds.Add(symbol.SymbolId);
                }

                if (symbol.MachineId == 1 && catalog.Machines.TryGetValue(2, out machine))
                {
                    machine.SymbolIds.Add(symbol.SymbolId);
                }
            }

            foreach (SlotPaylineConfig row in fiber.GetSingleton<SlotPaylineConfigCategory>().GetAll().Values)
            {
                SlotPaylineDef line = new SlotPaylineDef
                {
                    LineId = row.LineId,
                    Rows = Parse(row.Rows),
                    Columns = Parse(row.Columns),
                };
                catalog.Machines[1].Paylines.Add(line);
                catalog.Machines[2].Paylines.Add(new SlotPaylineDef { LineId = line.LineId, Rows = line.Rows, Columns = line.Columns });
            }

            catalog.Machines[1].Paylines.Sort((left, right) => left.LineId.CompareTo(right.LineId));
            catalog.Machines[2].Paylines.Sort((left, right) => left.LineId.CompareTo(right.LineId));

            foreach (SlotFreeSpinConfig row in fiber.GetSingleton<SlotFreeSpinConfigCategory>().GetAll().Values)
            {
                catalog.Machines[row.MachineId].FreeSpinsByCount[row.Count] = row.Spins;
            }

            foreach (SlotRoomConfig row in fiber.GetSingleton<SlotRoomConfigCategory>().GetAll().Values)
            {
                catalog.Rooms[row.Id] = new SlotRoomDef
                {
                    RoomId = row.Id,
                    MachineId = row.MachineId,
                    ProgressGroup = row.ProgressGroup,
                    MaxLines = row.MaxLines,
                    PublicStockMilli = row.PublicStockMilli,
                    PublicStockBase = (SlotLedgerBaseKind)row.PublicStockBase,
                    JackpotMilli = row.JackpotMilli,
                    JackpotBase = (SlotLedgerBaseKind)row.JackpotBase,
                    RankPoolMilli = row.RankPoolMilli,
                    RankPoolBase = (SlotLedgerBaseKind)row.RankPoolBase,
                    PoolWarning = row.PoolWarning,
                    InitialPublicStock = row.InitialPublicStock,
                };
            }

            foreach (SlotStakeConfig row in fiber.GetSingleton<SlotStakeConfigCategory>().GetAll().Values)
            {
                catalog.Stakes[row.Id] = new SlotStakeDef
                {
                    StakeId = row.Id,
                    RoomId = row.RoomId,
                    BetGold = row.BetGold,
                    WinFactor = row.WinFactor,
                    MinDamage = row.MinDamage,
                    VipLevel = row.VipLevel,
                    TotalCost = row.TotalCost,
                };
            }

            foreach (SlotStageConfig row in fiber.GetSingleton<SlotStageConfigCategory>().GetAll().Values)
            {
                SlotStageDef stage = new SlotStageDef
                {
                    StageId = row.Id,
                    ProgressGroup = row.ProgressGroup,
                    Order = row.Order,
                    NextStageId = row.NextStageId,
                    HpMax = row.HpMax,
                    DamageCapMicro = row.DamageCapMicro,
                    FreeDamageCapMicro = row.FreeDamageCapMicro,
                };
                if (row.RewardCount > 0)
                {
                    stage.ClearReward.Add(new SlotItemGrant(row.RewardItemId, row.RewardCount));
                }

                catalog.Stages[row.Id] = stage;
            }

            foreach (SlotHitConfig row in fiber.GetSingleton<SlotHitConfigCategory>().GetAll().Values)
            {
                catalog.HitChances.Add(new SlotHitChanceDef
                {
                    MachineId = row.MachineId,
                    LineCount = row.LineCount,
                    WinRate = row.WinRate,
                    DamageRate = row.DamageRate,
                });
            }

            foreach (SlotPolicyConfig row in fiber.GetSingleton<SlotPolicyConfigCategory>().GetAll().Values)
            {
                catalog.Policies.Add(new SlotPolicyBand
                {
                    Policy = row.Policy,
                    AxisMin = row.AxisMin,
                    AxisMax = row.AxisMax,
                    BandMin = row.BandMin,
                    BandMax = row.BandMax,
                    Delta = row.Delta,
                    GateMin = row.GateMin,
                    GateMax = row.GateMax,
                    AlternateDelta = row.AlternateDelta,
                });
            }

            foreach (SlotPatternConfig row in fiber.GetSingleton<SlotPatternConfigCategory>().GetAll().Values)
            {
                catalog.Patterns.Add(new SlotForcedPattern
                {
                    MachineId = row.MachineId,
                    Purpose = row.Purpose,
                    SymbolId = row.SymbolId,
                    Count = row.Count,
                    Weight = row.Weight,
                });
            }

            foreach (SlotJackpotConfig row in fiber.GetSingleton<SlotJackpotConfigCategory>().GetAll().Values)
            {
                catalog.Jackpots.Add(new SlotJackpotRule
                {
                    RoomId = row.RoomId,
                    SymbolId = row.SymbolId,
                    MinCount = row.MinCount,
                    MinBet = row.MinBet,
                    Count = row.Count,
                    RateMilli = row.RateMilli,
                });
            }
        }

        private static int[] Parse(string text)
        {
            string[] parts = text.Split(',');
            int[] values = new int[parts.Length];
            for (int i = 0; i < parts.Length; ++i)
            {
                values[i] = int.Parse(parts[i]);
            }

            return values;
        }
    }
}
