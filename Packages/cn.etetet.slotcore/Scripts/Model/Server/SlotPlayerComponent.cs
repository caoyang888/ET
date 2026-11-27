using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public sealed class SlotPlayerComponent : Entity, IAwake
    {
        public Dictionary<long, EntityRef<SlotPlayer>> Players = new();
    }
}
