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
        }
    }
}
