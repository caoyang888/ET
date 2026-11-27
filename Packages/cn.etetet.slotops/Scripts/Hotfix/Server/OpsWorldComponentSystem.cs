namespace ET.Server
{
    [EntitySystemOf(typeof(OpsWorldComponent))]
    public static partial class OpsWorldComponentSystem
    {
        [EntitySystem]
        private static void Awake(this OpsWorldComponent self)
        {
        }
    }
}
