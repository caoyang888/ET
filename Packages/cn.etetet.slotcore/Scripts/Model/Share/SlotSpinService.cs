namespace ET
{
    [EnableClass]
    public static class SlotSpinService
    {
        public static SlotRoundResult Spin(SlotCatalog catalog, SlotPlayerState player, SlotSpinRequest request, SlotRoomLedgerBook ledgers, SlotSpinStore store)
        {
            SlotRoundResult cached;
            if (!string.IsNullOrEmpty(request.ClientSpinId) && store.TryGet(player.PlayerId, request.ClientSpinId, out cached))
            {
                return cached;
            }

            SlotRoundResult result = SlotRoundSettler.Settle(catalog, player, request, ledgers);
            if (result.Error == SlotFail.Ok && !string.IsNullOrEmpty(request.ClientSpinId))
            {
                store.Save(result);
            }

            return result;
        }
    }
}
