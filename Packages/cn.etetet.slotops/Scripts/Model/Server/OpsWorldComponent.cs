using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public sealed class OpsWorldComponent : Entity, IAwake
    {
        public OpsCatalog Catalog = new();
        public Dictionary<int, int> KingQuota = new();
        public DevPaymentPort Payment = new();
        public DevIdentityPort Identity = new();
        public DevPhoneBindPort Phone = new();
        public DevAdProofPort Ads = new();
        public DevChannelCoefficientPort Channel = new();
    }
}
