using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotSymbolDef
    {
        public int SymbolId;
        public int MachineId;
        public SlotSymbolKind Kind;
        public int Weight;
        public Dictionary<int, long> PayMilli = new Dictionary<int, long>();

        public long PayOf(int count)
        {
            long value;
            if (this.PayMilli.TryGetValue(count, out value))
            {
                return value;
            }

            return 0;
        }
    }
}
