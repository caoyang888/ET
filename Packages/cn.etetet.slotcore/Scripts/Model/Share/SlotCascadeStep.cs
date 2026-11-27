using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotCascadeStep
    {
        public int Index;
        public List<int> Cells = new List<int>();
        public List<SlotWinLine> Wins = new List<SlotWinLine>();
        public List<SlotFeatureAward> Features = new List<SlotFeatureAward>();
    }
}
