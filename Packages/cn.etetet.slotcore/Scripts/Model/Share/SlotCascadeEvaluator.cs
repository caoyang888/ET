using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public static class SlotCascadeEvaluator
    {
        public static List<SlotCascadeStep> Evaluate(SlotCatalog catalog, SlotMachineDef machine, int[] grid, SlotRng rng)
        {
            List<SlotCascadeStep> steps = new List<SlotCascadeStep>();
            int[] working = (int[])grid.Clone();
            int guard = machine.MaxCascade <= 0 ? 10 : machine.MaxCascade;
            for (int stepIndex = 0; stepIndex < guard; ++stepIndex)
            {
                SlotCascadeStep step = new SlotCascadeStep();
                step.Index = stepIndex;
                step.Cells.AddRange(working);
                List<int> remove = new List<int>();
                Dictionary<int, int> counts = Count(working);
                foreach (KeyValuePair<int, int> pair in counts)
                {
                    SlotSymbolDef symbol;
                    if (!catalog.Symbols.TryGetValue(pair.Key, out symbol) || symbol.Kind != SlotSymbolKind.Regular)
                    {
                        continue;
                    }

                    long milli = symbol.PayOf(pair.Value);
                    if (milli <= 0)
                    {
                        continue;
                    }

                    SlotWinLine win = new SlotWinLine();
                    win.SymbolId = pair.Key;
                    win.Count = pair.Value;
                    win.MilliMultiplier = milli;
                    for (int i = 0; i < working.Length; ++i)
                    {
                        if (working[i] != pair.Key)
                        {
                            continue;
                        }

                        win.Positions.Add(new SlotCellPos(i / machine.Columns, i % machine.Columns));
                        remove.Add(i);
                    }

                    step.Wins.Add(win);
                }

                AddSpecials(catalog, machine, working, step);
                steps.Add(step);
                if (remove.Count == 0)
                {
                    break;
                }

                for (int i = 0; i < remove.Count; ++i)
                {
                    working[remove[i]] = -1;
                }

                Collapse(working, machine.Rows, machine.Columns);
                Refill(catalog, machine, working, rng);
            }

            return steps;
        }

        private static void AddSpecials(SlotCatalog catalog, SlotMachineDef machine, int[] grid, SlotCascadeStep step)
        {
            Dictionary<int, int> counts = Count(grid);
            foreach (KeyValuePair<int, int> pair in counts)
            {
                SlotSymbolDef symbol;
                if (!catalog.Symbols.TryGetValue(pair.Key, out symbol))
                {
                    continue;
                }

                if (symbol.Kind == SlotSymbolKind.JackpotTrigger || symbol.Kind == SlotSymbolKind.CascadeMultiplier)
                {
                    step.Features.Add(new SlotFeatureAward { Kind = symbol.Kind, SymbolId = pair.Key, Count = pair.Value });
                }
            }
        }

        private static Dictionary<int, int> Count(int[] grid)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();
            for (int i = 0; i < grid.Length; ++i)
            {
                if (grid[i] <= 0)
                {
                    continue;
                }

                int count;
                counts.TryGetValue(grid[i], out count);
                counts[grid[i]] = count + 1;
            }

            return counts;
        }

        private static void Collapse(int[] grid, int rows, int cols)
        {
            for (int col = 0; col < cols; ++col)
            {
                int write = rows - 1;
                for (int row = rows - 1; row >= 0; --row)
                {
                    int id = grid[row * cols + col];
                    if (id < 0)
                    {
                        continue;
                    }

                    grid[write * cols + col] = id;
                    if (write != row)
                    {
                        grid[row * cols + col] = -1;
                    }

                    --write;
                }
            }
        }

        private static void Refill(SlotCatalog catalog, SlotMachineDef machine, int[] grid, SlotRng rng)
        {
            List<int> ids = new List<int>();
            List<int> weights = new List<int>();
            for (int i = 0; i < machine.SymbolIds.Count; ++i)
            {
                SlotSymbolDef symbol = catalog.Symbol(machine.SymbolIds[i]);
                if (symbol.Weight <= 0 || symbol.Kind == SlotSymbolKind.JackpotTrigger)
                {
                    continue;
                }

                ids.Add(symbol.SymbolId);
                weights.Add(symbol.Weight);
            }

            for (int i = 0; i < grid.Length; ++i)
            {
                if (grid[i] >= 0)
                {
                    continue;
                }

                grid[i] = ids.Count == 0 ? 0 : ids[rng.Pick(weights)];
            }
        }
    }
}
