namespace ET
{
    [EnableClass]
    public sealed class SlotItemGrant
    {
        public int ItemId;
        public long Count;

        public SlotItemGrant()
        {
        }

        public SlotItemGrant(int itemId, long count)
        {
            this.ItemId = itemId;
            this.Count = count;
        }
    }
}
