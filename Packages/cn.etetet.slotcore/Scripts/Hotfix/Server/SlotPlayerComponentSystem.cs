namespace ET.Server
{
    [EntitySystemOf(typeof(SlotPlayerComponent))]
    public static partial class SlotPlayerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SlotPlayerComponent self)
        {
        }

        public static SlotPlayer Get(this SlotPlayerComponent self, long playerId)
        {
            EntityRef<SlotPlayer> slotRef;
            if (!self.Players.TryGetValue(playerId, out slotRef))
            {
                return null;
            }

            return slotRef;
        }
    }
}
