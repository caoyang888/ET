using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public static class SlotBoardGenerator
    {
        public static int[] Generate(SlotCatalog catalog, SlotMachineDef machine, bool win, int lineCount, SlotRng rng)
        {
            int[] grid = new int[machine.Rows * machine.Columns];
            List<SlotSymbolDef> regulars = Regulars(catalog, machine);
            if (regulars.Count == 0)
            {
                return grid;
            }

            for (int col = 0; col < machine.Columns; ++col)
            {
                SlotSymbolDef symbol = regulars[(col + (win ? 1 : 0)) % regulars.Count];
                for (int row = 0; row < machine.Rows; ++row)
                {
                    grid[row * machine.Columns + col] = symbol.SymbolId;
                }
            }

            if (!win || machine.Evaluator != SlotEvaluatorKind.Payline || machine.Paylines.Count == 0)
            {
                if (win && machine.Evaluator == SlotEvaluatorKind.Cascade)
                {
                    StampCluster(catalog, machine, grid, rng);
                }

                return grid;
            }

            SlotForcedPattern pattern = Pick(catalog, machine.MachineId, "Win", rng);
            int count = pattern == null ? 3 : pattern.Count;
            int symbolId = pattern == null ? regulars[0].SymbolId : pattern.SymbolId;
            int lineIndex = rng.NextInt(System.Math.Min(lineCount, machine.Paylines.Count));
            SlotPaylineDef line = machine.Paylines[lineIndex];
            int stamped = 0;
            for (int i = 0; i < line.Columns.Length && stamped < count; ++i)
            {
                grid[line.Rows[i] * machine.Columns + line.Columns[i]] = symbolId;
                ++stamped;
            }

            return grid;
        }

        private static void StampCluster(SlotCatalog catalog, SlotMachineDef machine, int[] grid, SlotRng rng)
        {
            SlotForcedPattern pattern = Pick(catalog, machine.MachineId, "Win", rng);
            if (pattern == null)
            {
                return;
            }

            int stamped = 0;
            for (int i = 0; i < grid.Length && stamped < pattern.Count; ++i)
            {
                grid[i] = pattern.SymbolId;
                ++stamped;
            }
        }

        private static List<SlotSymbolDef> Regulars(SlotCatalog catalog, SlotMachineDef machine)
        {
            List<SlotSymbolDef> list = new List<SlotSymbolDef>();
            for (int i = 0; i < machine.SymbolIds.Count; ++i)
            {
                SlotSymbolDef symbol = catalog.Symbol(machine.SymbolIds[i]);
                if (symbol.Kind == SlotSymbolKind.Regular && symbol.Weight > 0)
                {
                    list.Add(symbol);
                }
            }

            return list;
        }

        private static SlotForcedPattern Pick(SlotCatalog catalog, int machineId, string purpose, SlotRng rng)
        {
            List<SlotForcedPattern> rows = new List<SlotForcedPattern>();
            List<int> weights = new List<int>();
            foreach (SlotForcedPattern row in catalog.Patterns)
            {
                if (row.MachineId == machineId && row.Purpose == purpose && row.Weight > 0)
                {
                    rows.Add(row);
                    weights.Add(row.Weight);
                }
            }

            if (rows.Count == 0)
            {
                return null;
            }

            return rows[rng.Pick(weights)];
        }
    }
}
