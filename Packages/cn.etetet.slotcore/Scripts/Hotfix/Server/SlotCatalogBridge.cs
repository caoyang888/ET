namespace ET.Server
{
    public static class SlotCatalogBridge
    {
        public static SlotCatalog Load(Fiber fiber)
        {
            return SlotGeneratedCatalog.Load(fiber);
        }
    }
}
