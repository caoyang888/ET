namespace ET.Server
{
    [EntitySystemOf(typeof(SlotPlayer))]
    public static partial class SlotPlayerSystem
    {
        [EntitySystem]
        private static void Awake(this SlotPlayer self, long accountId)
        {
            self.AccountId = accountId;
            self.Spin.PlayerId = accountId;
            long jade = 0;
            SlotWorldComponent world = self.Root().GetComponent<SlotWorldComponent>();
            if (world != null && world.Catalog != null && world.Catalog.Rule != null)
            {
                jade = world.Catalog.Rule.NewPlayerJade;
                self.Spin.Add(world.Catalog.Rule.JadeItemId, jade);
            }
        }
    }
}
