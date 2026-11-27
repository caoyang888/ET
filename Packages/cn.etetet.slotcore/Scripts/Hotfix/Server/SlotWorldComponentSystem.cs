namespace ET.Server
{
    [EntitySystemOf(typeof(SlotWorldComponent))]
    public static partial class SlotWorldComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SlotWorldComponent self)
        {
        }
    }
}
