namespace ET
{
    [EnableClass]
    public sealed class SlotProgressState
    {
        public int StageId;
        public long BossHp;
        public int FreeSpins;
        public int LockedStakeId;
        public int LockedLineCount;
        public bool Cleared;
        public int MissStreak;
        public int PityCooldown;
    }
}
