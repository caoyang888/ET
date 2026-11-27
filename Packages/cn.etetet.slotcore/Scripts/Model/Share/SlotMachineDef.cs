using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotMachineDef
    {
        public int MachineId;
        public SlotEvaluatorKind Evaluator;
        public int Rows;
        public int Columns;
        public int MaxCascade;
        public int ProgressGroup;
        public List<int> SymbolIds = new List<int>();
        public List<SlotPaylineDef> Paylines = new List<SlotPaylineDef>();
        public Dictionary<int, int> FreeSpinsByCount = new Dictionary<int, int>();
        public List<SlotItemGrant> ClearReward = new List<SlotItemGrant>();
    }
}
