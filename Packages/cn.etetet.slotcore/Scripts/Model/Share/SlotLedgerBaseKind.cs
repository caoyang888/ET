namespace ET
{
    /// <summary>
    /// 房间账本计提基数。PublicStock 与 RankPool 用押注，Jackpot 用赢分。
    /// </summary>
    public enum SlotLedgerBaseKind
    {
        Bet = 1,
        Win = 2,
    }
}
