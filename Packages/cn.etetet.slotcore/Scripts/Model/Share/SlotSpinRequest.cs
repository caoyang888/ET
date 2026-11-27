namespace ET
{
    [EnableClass]
    public sealed class SlotSpinRequest
    {
        public long PlayerId;
        public string ClientSpinId;
        public int RoomId;
        public int StakeId;
        public int LineCount;
        public long Seed;
    }
}
