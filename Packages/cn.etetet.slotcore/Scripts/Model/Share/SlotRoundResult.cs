using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotRoundResult
    {
        public int Error;
        public long PlayerId;
        public string ClientSpinId;
        public int RoomId;
        public int StakeId;
        public int LineCount;
        public bool UsedFreeSpin;
        public long StakeGold;
        public long Payout;
        public long Damage;
        public int StageId;
        public long BossHp;
        public bool StageCleared;
        public int FreeSpinsLeft;
        public long Seed;
        public double HitChance;
        public bool ForcedByPity;
        public long PublicStockDelta;
        public long JackpotDelta;
        public long RankPoolDelta;
        public long JackpotTaken;
        public List<SlotCascadeStep> Steps = new List<SlotCascadeStep>();
        public List<SlotItemGrant> Grants = new List<SlotItemGrant>();
    }
}
