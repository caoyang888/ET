namespace ET.Server
{
    public static class OpsGeneratedCatalog
    {
        public static OpsCatalog Load(Fiber fiber)
        {
            OpsCatalog catalog = new OpsCatalog();
            OpsGeneratedCatalogFill.Fill(fiber, catalog);
            return catalog;
        }
    }
}
