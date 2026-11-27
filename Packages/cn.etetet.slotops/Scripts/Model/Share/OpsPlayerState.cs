using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class OpsMail
    {
        public long MailId;
        public string Title;
        public bool Read;
        public bool Claimed;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsOrder
    {
        public string OrderId;
        public int ProductId;
        public bool Paid;
        public bool Granted;
    }

    [EnableClass]
    public sealed class OpsPlayerState
    {
        public int VipExp;
        public int JubaoLevel = 1;
        public long JubaoReadyUnix;
        public int DiceCell = 1;
        public int DiceLaps;
        public long Piggy;
        public long PeriodDamage;
        public int BankruptcyClaims;
        public bool RealNameVerified;
        public bool PhoneBound;
        public long NextMailId = 1;
        public Dictionary<int, long> TaskProgress = new Dictionary<int, long>();
        public HashSet<int> ClaimedTasks = new HashSet<int>();
        public HashSet<int> ClaimedLaps = new HashSet<int>();
        public HashSet<int> ClaimedKing = new HashSet<int>();
        public HashSet<string> Redeemed = new HashSet<string>();
        public Dictionary<int, int> PurchaseCount = new Dictionary<int, int>();
        public Dictionary<int, int> KingTaken = new Dictionary<int, int>();
        public Dictionary<string, OpsOrder> Orders = new Dictionary<string, OpsOrder>();
        public List<OpsMail> Mail = new List<OpsMail>();

        public long Progress(int condition)
        {
            long value;
            this.TaskProgress.TryGetValue(condition, out value);
            return value;
        }

        public void AddProgress(int condition, long amount)
        {
            this.TaskProgress[condition] = this.Progress(condition) + amount;
        }
    }
}
