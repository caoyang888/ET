using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotRuleDef
    {
        public int GoldItemId = 2;
        public int JadeItemId = 1;
        public long NewPlayerJade = 15000;
        public int LimitDamageRate = 3;
        public long DamagePrice = 400000;
        public int PiggyMilli = 5;
        public List<SlotMultiplierFace> MultiplierFaces = new List<SlotMultiplierFace>();
    }
}
