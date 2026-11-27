namespace ET.Server
{
    public static class SlotCatalogLoader
    {
        public static SlotCatalog Load(Fiber fiber)
        {
            return SlotCatalogBridge.Load(fiber);
        }
    }
}
