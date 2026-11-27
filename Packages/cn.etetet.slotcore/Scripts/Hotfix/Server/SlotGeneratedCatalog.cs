namespace ET.Server
{
    public static class SlotGeneratedCatalog
    {
        public static SlotCatalog Load(Fiber fiber)
        {
            SlotCatalog catalog = new SlotCatalog();
            SlotGeneratedCatalogFill.Fill(fiber, catalog);
            return catalog;
        }
    }
}
