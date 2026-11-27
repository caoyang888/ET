namespace ET.Server
{
    [ChildOf(typeof(SlotPlayerComponent))]
    public sealed class SlotPlayer : Entity, IAwake<long>
    {
        public long AccountId;
        public string Account = string.Empty;
        public string Nickname = string.Empty;
        public string Avatar = string.Empty;
        public string GateIp = string.Empty;
        public int GatePort;
        public long GateFiber;
        public long GateInstance;
        public SlotPlayerState Spin = new();
    }
}
