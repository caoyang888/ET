using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotCatalog
    {
        public SlotRuleDef Rule = new SlotRuleDef();
        public SlotPityDef Pity = new SlotPityDef();
        public Dictionary<int, SlotSymbolDef> Symbols = new Dictionary<int, SlotSymbolDef>();
        public Dictionary<int, SlotMachineDef> Machines = new Dictionary<int, SlotMachineDef>();
        public Dictionary<int, SlotRoomDef> Rooms = new Dictionary<int, SlotRoomDef>();
        public Dictionary<int, SlotStakeDef> Stakes = new Dictionary<int, SlotStakeDef>();
        public Dictionary<int, SlotStageDef> Stages = new Dictionary<int, SlotStageDef>();
        public List<SlotHitChanceDef> HitChances = new List<SlotHitChanceDef>();
        public List<SlotPolicyBand> Policies = new List<SlotPolicyBand>();
        public List<SlotForcedPattern> Patterns = new List<SlotForcedPattern>();
        public List<SlotJackpotRule> Jackpots = new List<SlotJackpotRule>();

        public SlotSymbolDef Symbol(int id)
        {
            return this.Symbols[id];
        }

        public SlotStageDef FirstStage(int progressGroup)
        {
            SlotStageDef best = null;
            foreach (SlotStageDef stage in this.Stages.Values)
            {
                if (stage.ProgressGroup != progressGroup)
                {
                    continue;
                }

                if (best == null || stage.Order < best.Order)
                {
                    best = stage;
                }
            }

            return best;
        }
    }
}
