using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public static class SlotPaylineEvaluator
    {
        public static void Evaluate(SlotCatalog catalog, SlotMachineDef machine, int[] grid, int lineCount, List<SlotWinLine> wins, List<SlotFeatureAward> features)
        {
            int cols = machine.Columns;
            int used = lineCount;
            if (used > machine.Paylines.Count)
            {
                used = machine.Paylines.Count;
            }

            if (used < 1)
            {
                used = 1;
            }

            for (int i = 0; i < used; ++i)
            {
                SlotPaylineDef line = machine.Paylines[i];
                int symbolId = At(grid, cols, line, 0);
                SlotSymbolDef symbol;
                if (!catalog.Symbols.TryGetValue(symbolId, out symbol) || symbol.Kind != SlotSymbolKind.Regular)
                {
                    continue;
                }

                int count = 1;
                while (count < line.Columns.Length && At(grid, cols, line, count) == symbolId)
                {
                    ++count;
                }

                long milli = symbol.PayOf(count);
                if (milli <= 0)
                {
                    continue;
                }

                SlotWinLine win = new SlotWinLine();
                win.LineIndex = line.LineId;
                win.SymbolId = symbolId;
                win.Count = count;
                win.MilliMultiplier = milli;
                for (int k = 0; k < count; ++k)
                {
                    win.Positions.Add(new SlotCellPos(line.Rows[k], line.Columns[k]));
                }

                wins.Add(win);
            }

            AddScatterFeatures(catalog, machine, grid, features);
        }

        public static void AddScatterFeatures(SlotCatalog catalog, SlotMachineDef machine, int[] grid, List<SlotFeatureAward> features)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            bool[] reelHasFeature = new bool[machine.Columns];
            for (int row = 0; row < machine.Rows; ++row)
            {
                for (int col = 0; col < machine.Columns; ++col)
                {
                    int id = grid[row * machine.Columns + col];
                    int count;
                    counts.TryGetValue(id, out count);
                    counts[id] = count + 1;
                    SlotSymbolDef symbol;
                    if (catalog.Symbols.TryGetValue(id, out symbol) && symbol.Kind == SlotSymbolKind.LineFeature)
                    {
                        reelHasFeature[col] = true;
                    }
                }
            }

            foreach (KeyValuePair<int, int> pair in counts)
            {
                SlotSymbolDef symbol;
                if (!catalog.Symbols.TryGetValue(pair.Key, out symbol))
                {
                    continue;
                }

                if (symbol.Kind == SlotSymbolKind.FreeSpin && machine.FreeSpinsByCount.ContainsKey(pair.Value))
                {
                    features.Add(new SlotFeatureAward { Kind = symbol.Kind, SymbolId = pair.Key, Count = pair.Value });
                }

                if (symbol.Kind == SlotSymbolKind.JackpotCounter)
                {
                    features.Add(new SlotFeatureAward { Kind = symbol.Kind, SymbolId = pair.Key, Count = pair.Value });
                }
            }

            int run = 0;
            int best = 0;
            for (int col = 0; col < reelHasFeature.Length; ++col)
            {
                run = reelHasFeature[col] ? run + 1 : 0;
                if (run > best)
                {
                    best = run;
                }
            }

            if (best >= 3)
            {
                foreach (SlotSymbolDef symbol in catalog.Symbols.Values)
                {
                    if (symbol.MachineId == machine.MachineId && symbol.Kind == SlotSymbolKind.LineFeature)
                    {
                        features.Add(new SlotFeatureAward
                        {
                            Kind = SlotSymbolKind.LineFeature,
                            SymbolId = symbol.SymbolId,
                            Count = best,
                            FaceMilli = (int)symbol.PayOf(best),
                        });
                        break;
                    }
                }
            }
        }

        private static int At(int[] grid, int cols, SlotPaylineDef line, int index)
        {
            return grid[line.Rows[index] * cols + line.Columns[index]];
        }
    }
}
