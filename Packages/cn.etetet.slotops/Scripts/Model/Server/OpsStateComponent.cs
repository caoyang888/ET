namespace ET.Server
{
    [ComponentOf(typeof(SlotPlayer))]
    public sealed class OpsStateComponent : Entity, IAwake
    {
        public OpsPlayerState State = new();
    }
}
