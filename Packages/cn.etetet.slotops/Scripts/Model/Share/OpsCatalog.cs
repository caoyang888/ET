using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class OpsProductDef
    {
        public int ProductId;
        public int Category;
        public long PriceCents;
        public int LimitCount;
        public int VipExp;
        public bool CountsTowardJubao;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsTaskDef
    {
        public int TaskId;
        public int TaskType;
        public int Condition;
        public long Target;
        public int Param;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsVipDef
    {
        public int Level;
        public int Exp;
        public long PiggyMax;
        public bool CanIdle;
        public bool CanRename;
        public bool CanService;
    }

    [EnableClass]
    public sealed class OpsJubaoDef
    {
        public int Level;
        public long Gold;
        public int RechargeCents;
        public int CostItemId;
        public long CostCount;
        public int CooldownSeconds;
    }

    [EnableClass]
    public sealed class OpsDiceDef
    {
        public int Cell;
        public int Weight;
        public int LuckyWeight;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsLapDef
    {
        public int Laps;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsRankDef
    {
        public int RankId;
        public int Period;
        public int Place;
        public long RivalStep;
        public long StopAtPlayerDamage;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsKingDef
    {
        public int TierId;
        public long TargetDamage;
        public int Quota;
        public List<SlotItemGrant> Rewards = new List<SlotItemGrant>();
    }

    [EnableClass]
    public sealed class OpsActivityDef
    {
        public int ActivityId;
        public int Kind;
        public long BeginUnix;
        public long EndUnix;
    }

    [EnableClass]
    public sealed class OpsCatalog
    {
        public List<OpsProductDef> Products = new List<OpsProductDef>();
        public List<OpsTaskDef> Tasks = new List<OpsTaskDef>();
        public List<OpsVipDef> Vip = new List<OpsVipDef>();
        public List<OpsJubaoDef> Jubao = new List<OpsJubaoDef>();
        public List<OpsDiceDef> Dice = new List<OpsDiceDef>();
        public List<OpsLapDef> Laps = new List<OpsLapDef>();
        public List<OpsRankDef> Ranks = new List<OpsRankDef>();
        public List<OpsKingDef> King = new List<OpsKingDef>();
        public List<OpsActivityDef> Activities = new List<OpsActivityDef>();
        public Dictionary<string, List<SlotItemGrant>> RedeemCodes = new Dictionary<string, List<SlotItemGrant>>();
        public int BankruptcyPerDay = 2;
        public long BankruptcyGold = 10000;
        public int PiggyProductId = 301;
    }
}
