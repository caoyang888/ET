namespace ET.Server
{
    [EntitySystemOf(typeof(OpsStateComponent))]
    public static partial class OpsStateComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OpsStateComponent self)
        {
        }
    }
}
