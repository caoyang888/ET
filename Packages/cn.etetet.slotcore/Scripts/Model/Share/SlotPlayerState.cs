using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotPlayerState
    {
        public long PlayerId;
        public Dictionary<int, long> Wallet = new Dictionary<int, long>();
        public Dictionary<int, SlotProgressState> Progress = new Dictionary<int, SlotProgressState>();
        public long TotalBet;
        public long TotalWin;
        public long TotalDamage;
        public int RechargeCents;
        public long TotalCost;

        public long Get(int itemId)
        {
            long value;
            this.Wallet.TryGetValue(itemId, out value);
            return value;
        }

        public void Add(int itemId, long count)
        {
            this.Wallet[itemId] = this.Get(itemId) + count;
        }
    }
}
