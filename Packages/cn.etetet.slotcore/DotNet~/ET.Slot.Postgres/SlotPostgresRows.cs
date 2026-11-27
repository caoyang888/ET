using System.Collections.Generic;

namespace ET.Slot.Postgres
{
    public sealed class SlotAccountRow
    {
        public string Account;
        public long PlayerId;
        public string Nickname;
        public string Avatar;
        public string WalletJson;
        public int VipExp;
        public bool RealNameVerified;
        public bool PhoneBound;
    }

    public sealed class SlotProgressRow
    {
        public int ProgressGroup;
        public int StageId;
        public long BossHp;
        public bool Cleared;
        public int FreeSpins;
        public int LockedStakeId;
        public int LockedLineCount;
        public int MissStreak;
        public int PityCooldown;
        public long TotalBet;
        public long TotalWin;
        public long TotalDamage;
        public int RechargeCents;
        public long TotalCost;
        public int ClearCount;
    }

    public sealed class SlotLedgerRow
    {
        public int RoomId;
        public long PublicStock;
        public long RankPool;
        public long JackpotPool;
        public long Version;
    }

    public sealed class SlotSpinRow
    {
        public string ClientSpinId;
        public long PlayerId;
        public int RoomId;
        public int StakeId;
        public long Bet;
        public long Win;
        public long Damage;
        public long Seed;
        public string ResultJson;
        public long CreatedUnix;
    }

    public sealed class SlotOpsRow
    {
        public int JubaoLevel;
        public long JubaoReadyUnix;
        public int DiceCell;
        public int DiceLaps;
        public long Piggy;
        public int BankruptcyClaims;
        public string TaskJson;
        public string ClaimJson;
        public string KingJson;
        public string LapJson;
        public string RedeemJson;
    }

    public sealed class SlotOrderRow
    {
        public string OrderId;
        public long PlayerId;
        public int ProductId;
        public long Amount;
        public string Status;
        public string Receipt;
        public long CreatedUnix;
    }

    public sealed class SlotMailRow
    {
        public long MailId;
        public long PlayerId;
        public string Title;
        public string Body;
        public string AttachmentJson;
        public bool IsRead;
        public bool IsClaimed;
        public bool IsDeleted;
        public long ExpireUnix;
    }

    public sealed class SlotRankRow
    {
        public long PlayerId;
        public string Nickname;
        public int Period;
        public long Damage;
    }

    public sealed class SlotSnapshot
    {
        public SlotAccountRow Account;
        public List<SlotProgressRow> Progress = new List<SlotProgressRow>();
        public SlotOpsRow Ops;
        public List<SlotOrderRow> Orders = new List<SlotOrderRow>();
        public List<SlotMailRow> Mails = new List<SlotMailRow>();
        public List<SlotRankRow> Ranks = new List<SlotRankRow>();
    }
}
