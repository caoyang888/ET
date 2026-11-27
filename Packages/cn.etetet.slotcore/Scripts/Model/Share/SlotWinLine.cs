using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotWinLine
    {
        public int LineIndex;
        public int SymbolId;
        public int Count;
        public long MilliMultiplier;
        public long Amount;
        public List<SlotCellPos> Positions = new List<SlotCellPos>();
    }
}
