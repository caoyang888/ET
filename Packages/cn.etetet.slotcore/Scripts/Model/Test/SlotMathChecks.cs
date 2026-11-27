using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET.Test
{
    [EnableClass]
    public static class SlotMathChecks
    {
        public static int Payline()
        {
            SlotCatalog catalog = LineCatalog();
            int[] grid = new int[15];
            for (int i = 0; i < grid.Length; ++i)
            {
                grid[i] = 2;
            }

            for (int col = 0; col < 5; ++col)
            {
                grid[5 + col] = 1;
            }

            List<SlotWinLine> wins = new List<SlotWinLine>();
            List<SlotFeatureAward> features = new List<SlotFeatureAward>();
            SlotPaylineEvaluator.Evaluate(catalog, catalog.Machines[1], grid, 1, wins, features);
            if (wins.Count != 1 || wins[0].Count != 5 || wins[0].Positions.Count != 5)
            {
                return 1;
            }

            if (wins[0].Positions[0].Row != 1 || wins[0].Positions[4].Column != 4)
            {
                return 2;
            }

            return 0;
        }

        public static int FreeSpinAndFeatures()
        {
            SlotCatalog catalog = LineCatalog();
            SlotPlayerState player = new SlotPlayerState();
            player.PlayerId = 7;
            player.Add(2, 100000);
            SlotSpinRequest request = new SlotSpinRequest
            {
                PlayerId = 7,
                ClientSpinId = "free-1",
                RoomId = 101,
                StakeId = 10101,
                LineCount = 1,
                Seed = 11,
            };
            SlotRoundResult result = PlayFixed(catalog, player, request, ThreeNines());
            if (result.Error != 0 || result.FreeSpinsLeft != 3)
            {
                return 3;
            }

            int[] featureGrid = new int[15];
            for (int i = 0; i < featureGrid.Length; ++i)
            {
                featureGrid[i] = 2;
            }

            featureGrid[0] = 10;
            featureGrid[1] = 10;
            featureGrid[2] = 10;
            featureGrid[3] = 11;
            List<SlotWinLine> wins = new List<SlotWinLine>();
            List<SlotFeatureAward> features = new List<SlotFeatureAward>();
            SlotPaylineEvaluator.Evaluate(catalog, catalog.Machines[1], featureGrid, 1, wins, features);
            bool lineFeature = false;
            bool counter = false;
            for (int i = 0; i < features.Count; ++i)
            {
                if (features[i].Kind == SlotSymbolKind.LineFeature && features[i].Count >= 3)
                {
                    lineFeature = true;
                }

                if (features[i].Kind == SlotSymbolKind.JackpotCounter && features[i].SymbolId == 11)
                {
                    counter = true;
                }
            }

            return lineFeature && counter ? 0 : 4;
        }

        public static int CascadeCoordinates()
        {
            SlotCatalog catalog = CascadeCatalog();
            int[] grid = new int[30];
            for (int i = 0; i < 8; ++i)
            {
                grid[i] = 21;
            }

            grid[8] = 32;
            List<SlotCascadeStep> steps = SlotCascadeEvaluator.Evaluate(catalog, catalog.Machines[3], grid, new SlotRng(3));
            if (steps.Count < 2 || steps[0].Wins.Count != 1 || steps[0].Wins[0].Positions.Count != 8)
            {
                return 5;
            }

            if (steps[0].Cells.Count != 30 || steps[0].Wins[0].Positions[0].Column != 0)
            {
                return 6;
            }

            return 0;
        }

        public static int RtpAndPity()
        {
            SlotCatalog catalog = LineCatalog();
            catalog.HitChances[0].WinRate = 0.1;
            catalog.Policies.Add(new SlotPolicyBand { Policy = "NewBalance", AxisMin = 0, AxisMax = 100000, BandMin = 0, BandMax = 10, Delta = 0.2 });
            SlotPlayerState player = new SlotPlayerState();
            player.Add(2, 100);
            double chance = SlotRtpComposer.Compose(catalog, player, catalog.Rooms[101], 1, 0, new SlotRng(1));
            if (chance < 0.29 || chance > 0.31)
            {
                return 7;
            }

            player.Add(2, 100000);
            player.Progress[1] = new SlotProgressState { StageId = 1001, BossHp = 50000, MissStreak = 5 };
            catalog.Pity.MissThreshold = 5;
            SlotRoundResult forced = SlotRoundSettler.Settle(catalog, player, new SlotSpinRequest
            {
                PlayerId = 1,
                ClientSpinId = "pity",
                RoomId = 101,
                StakeId = 10101,
                LineCount = 1,
                Seed = 99,
            }, new SlotRoomLedgerBook());
            if (!forced.ForcedByPity || forced.Payout <= 0)
            {
                return 8;
            }

            return 0;
        }

        public static int DamageCap()
        {
            SlotCatalog catalog = LineCatalog();
            catalog.Stages[1001].HpMax = 100000;
            catalog.Stages[1001].DamageCapMicro = 10000;
            catalog.Rule.LimitDamageRate = 100000;
            SlotStakeDef stake = catalog.Stakes[10101];
            long damage = SlotDamageResolver.Resolve(catalog, stake, catalog.Stages[1001], 5000000, 300, false);
            if (damage != 1000)
            {
                return 9;
            }

            long floor = SlotDamageResolver.Resolve(catalog, stake, catalog.Stages[1001], 0, 300, false);
            if (floor != stake.MinDamage)
            {
                return 10;
            }

            return 0;
        }

        public static int IdempotentSpin()
        {
            SlotCatalog catalog = LineCatalog();
            SlotPlayerState player = new SlotPlayerState();
            player.PlayerId = 4;
            player.Add(2, 10000);
            SlotSpinStore store = new SlotSpinStore();
            SlotRoomLedgerBook book = new SlotRoomLedgerBook();
            SlotSpinRequest request = new SlotSpinRequest
            {
                PlayerId = 4,
                ClientSpinId = "same",
                RoomId = 101,
                StakeId = 10101,
                LineCount = 1,
                Seed = 5,
            };
            SlotRoundResult first = SlotSpinService.Spin(catalog, player, request, book, store);
            long gold = player.Get(2);
            SlotRoundResult second = SlotSpinService.Spin(catalog, player, request, book, store);
            if (first.Error != 0 || second.Payout != first.Payout || player.Get(2) != gold)
            {
                return 11;
            }

            return 0;
        }

        public static int LedgerAndJackpot()
        {
            SlotRoomLedgerBook book = new SlotRoomLedgerBook();
            SlotRoomDef room = new SlotRoomDef
            {
                RoomId = 203,
                PublicStockMilli = 900,
                PublicStockBase = SlotLedgerBaseKind.Bet,
                JackpotMilli = 60,
                JackpotBase = SlotLedgerBaseKind.Win,
                RankPoolMilli = 40,
                RankPoolBase = SlotLedgerBaseKind.Bet,
            };
            Parallel.For(0, 100, _ =>
            {
                SlotRoundResult row = new SlotRoundResult();
                book.Apply(room, 1000, 1000, row);
            });
            SlotRoomLedger snap = book.Snapshot(203);
            if (snap.PublicStock != 90000 || snap.RankPool != 4000 || snap.Jackpot != 6000 || snap.Version != 100)
            {
                return 12;
            }

            long taken = book.TryTakeJackpot(203, 500);
            if (taken != 3000 || book.Snapshot(203).Jackpot != 3000)
            {
                return 13;
            }

            return 0;
        }

        private static SlotRoundResult PlayFixed(SlotCatalog catalog, SlotPlayerState player, SlotSpinRequest request, int[] grid)
        {
            SlotMachineDef machine = catalog.Machines[1];
            SlotCascadeStep step = new SlotCascadeStep();
            for (int i = 0; i < grid.Length; ++i)
            {
                step.Cells.Add(grid[i]);
            }

            SlotPaylineEvaluator.Evaluate(catalog, machine, grid, request.LineCount, step.Wins, step.Features);
            int grant = 0;
            for (int i = 0; i < step.Features.Count; ++i)
            {
                int value;
                if (step.Features[i].Kind == SlotSymbolKind.FreeSpin && machine.FreeSpinsByCount.TryGetValue(step.Features[i].Count, out value))
                {
                    grant = value;
                }
            }

            SlotProgressState progress = new SlotProgressState { StageId = 1001, BossHp = 50000, FreeSpins = grant };
            player.Progress[1] = progress;
            return new SlotRoundResult { Error = 0, FreeSpinsLeft = grant };
        }

        private static int[] ThreeNines()
        {
            int[] grid = new int[15];
            for (int i = 0; i < grid.Length; ++i)
            {
                grid[i] = 2;
            }

            grid[0] = 9;
            grid[6] = 9;
            grid[12] = 9;
            return grid;
        }

        private static SlotCatalog LineCatalog()
        {
            SlotCatalog catalog = new SlotCatalog();
            catalog.Machines[1] = new SlotMachineDef
            {
                MachineId = 1,
                Evaluator = SlotEvaluatorKind.Payline,
                Rows = 3,
                Columns = 5,
                ProgressGroup = 1,
                SymbolIds = new List<int> { 1, 2, 9, 10, 11 },
                Paylines = new List<SlotPaylineDef>
                {
                    new SlotPaylineDef { LineId = 1, Rows = new[] { 1, 1, 1, 1, 1 }, Columns = new[] { 0, 1, 2, 3, 4 } },
                },
                FreeSpinsByCount = new Dictionary<int, int> { { 3, 3 }, { 4, 4 }, { 5, 5 } },
            };
            catalog.Symbols[1] = new SlotSymbolDef
            {
                SymbolId = 1,
                MachineId = 1,
                Kind = SlotSymbolKind.Regular,
                Weight = 10,
                PayMilli = new Dictionary<int, long> { { 5, 75000 } },
            };
            catalog.Symbols[2] = new SlotSymbolDef { SymbolId = 2, MachineId = 1, Kind = SlotSymbolKind.Regular, Weight = 10 };
            catalog.Symbols[9] = new SlotSymbolDef { SymbolId = 9, MachineId = 1, Kind = SlotSymbolKind.FreeSpin, Weight = 1 };
            catalog.Symbols[10] = new SlotSymbolDef
            {
                SymbolId = 10,
                MachineId = 1,
                Kind = SlotSymbolKind.LineFeature,
                PayMilli = new Dictionary<int, long> { { 3, 25000 } },
            };
            catalog.Symbols[11] = new SlotSymbolDef { SymbolId = 11, MachineId = 1, Kind = SlotSymbolKind.JackpotCounter };
            catalog.Rooms[101] = new SlotRoomDef { RoomId = 101, MachineId = 1, ProgressGroup = 1, MaxLines = 9 };
            catalog.Stakes[10101] = new SlotStakeDef { StakeId = 10101, RoomId = 101, BetGold = 300, WinFactor = 1, MinDamage = 5 };
            catalog.Stages[1001] = new SlotStageDef { StageId = 1001, ProgressGroup = 1, Order = 1, HpMax = 50000, DamageCapMicro = 1000000 };
            catalog.Patterns.Add(new SlotForcedPattern { MachineId = 1, Purpose = "Win", SymbolId = 1, Count = 5, Weight = 1 });
            catalog.HitChances.Add(new SlotHitChanceDef { MachineId = 1, LineCount = 1, WinRate = 0 });
            return catalog;
        }

        private static SlotCatalog CascadeCatalog()
        {
            SlotCatalog catalog = new SlotCatalog();
            catalog.Machines[3] = new SlotMachineDef
            {
                MachineId = 3,
                Evaluator = SlotEvaluatorKind.Cascade,
                Rows = 5,
                Columns = 6,
                MaxCascade = 10,
                SymbolIds = new List<int> { 21, 32 },
            };
            catalog.Symbols[21] = new SlotSymbolDef
            {
                SymbolId = 21,
                MachineId = 3,
                Kind = SlotSymbolKind.Regular,
                PayMilli = new Dictionary<int, long> { { 8, 250 } },
            };
            catalog.Symbols[32] = new SlotSymbolDef { SymbolId = 32, MachineId = 3, Kind = SlotSymbolKind.CascadeMultiplier };
            catalog.Rule.MultiplierFaces.Add(new SlotMultiplierFace { Face = 2, Weight = 1 });
            return catalog;
        }
    }
}
