namespace ET
{
    [EnableClass]
    public sealed class SlotRoomDef
    {
        public int RoomId;
        public int MachineId;
        public int ProgressGroup;
        public int MaxLines;
        /// <summary>公共库存计提千分比，基数是押注。</summary>
        public int PublicStockMilli;
        public SlotLedgerBaseKind PublicStockBase;
        /// <summary>大奖池计提千分比，基数是赢分。</summary>
        public int JackpotMilli;
        public SlotLedgerBaseKind JackpotBase;
        /// <summary>排行资金计提千分比，基数是押注。</summary>
        public int RankPoolMilli;
        public SlotLedgerBaseKind RankPoolBase;
        public long PoolWarning;
        public long InitialPublicStock;
    }
}
