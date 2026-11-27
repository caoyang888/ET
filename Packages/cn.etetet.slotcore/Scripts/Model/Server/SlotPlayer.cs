namespace ET.Server
{
    [ChildOf(typeof(SlotPlayerComponent))]
    public sealed class SlotPlayer : Entity, IAwake<long>
    {
        public long AccountId;
        public SlotPlayerState Spin = new();
    }
}
