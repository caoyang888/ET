using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotStageDef
    {
        public int StageId;
        public int ProgressGroup;
        public int Order;
        public int NextStageId;
        public long HpMax;
        /// <summary>付费旋转单次伤害不超过最大生命的百万分比。1000000 表示 100%。</summary>
        public int DamageCapMicro;
        /// <summary>免费旋转单次伤害上限，单位同上。0 表示沿用付费上限。</summary>
        public int FreeDamageCapMicro;
        public List<SlotItemGrant> ClearReward = new List<SlotItemGrant>();
    }
}
