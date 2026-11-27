using System.Net;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class WebSocketOuterComponent: Entity, IAwake<IPEndPoint>, IDestroy, IUpdate
    {
        public AService AService { get; set; }
    }
}
