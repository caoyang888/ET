using System.Collections.Generic;

namespace ET.Server
{
    public static class SlotProtoMap
    {
        public static void FillWallet(List<SlotGrantProto> target, SlotPlayerState state)
        {
            target.Clear();
            foreach (KeyValuePair<int, long> pair in state.Wallet)
            {
                SlotGrantProto grant = SlotGrantProto.Create();
                grant.ItemId = pair.Key;
                grant.Count = pair.Value;
                target.Add(grant);
            }
        }

        public static void FillSteps(List<SlotStepProto> target, SlotRoundResult result)
        {
            target.Clear();
            foreach (SlotCascadeStep step in result.Steps)
            {
                SlotStepProto proto = SlotStepProto.Create();
                proto.Index = step.Index;
                for (int i = 0; i < step.Cells.Count; ++i)
                {
                    proto.Symbols.Add(step.Cells[i]);
                }

                foreach (SlotWinLine win in step.Wins)
                {
                    SlotWinProto winProto = SlotWinProto.Create();
                    winProto.SymbolId = win.SymbolId;
                    winProto.Count = win.Count;
                    winProto.Amount = win.Amount;
                    foreach (SlotCellPos pos in win.Positions)
                    {
                        SlotCellProto cell = SlotCellProto.Create();
                        cell.Row = pos.Row;
                        cell.Column = pos.Column;
                        cell.SymbolId = win.SymbolId;
                        winProto.Cells.Add(cell);
                    }

                    proto.Wins.Add(winProto);
                }

                target.Add(proto);
            }
        }

        public static void FillGrants(List<SlotGrantProto> target, List<SlotItemGrant> grants)
        {
            target.Clear();
            for (int i = 0; i < grants.Count; ++i)
            {
                SlotGrantProto grant = SlotGrantProto.Create();
                grant.ItemId = grants[i].ItemId;
                grant.Count = grants[i].Count;
                target.Add(grant);
            }
        }
    }
}
