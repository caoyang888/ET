using System.Net;

namespace ET
{
    [EntitySystemOf(typeof(WebSocketOuterComponent))]
    public static partial class WebSocketOuterComponentSystem
    {
        [EntitySystem]
        private static void Awake(this WebSocketOuterComponent self, IPEndPoint address)
        {
            WebSocketServerConfig config = WebSocketServerConfig.Load();
            self.AService = new KService(new WebSocketTransport(address, config), ServiceType.Outer);
            self.AService.AcceptCallback = self.OnAccept;
            self.AService.ReadCallback = self.OnRead;
            self.AService.ErrorCallback = self.OnError;
        }

        [EntitySystem]
        private static void Update(this WebSocketOuterComponent self)
        {
            self.AService.Update();
        }

        [EntitySystem]
        private static void Destroy(this WebSocketOuterComponent self)
        {
            self.AService.Dispose();
        }

        private static void OnError(this WebSocketOuterComponent self, long channelId, int error)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }

            session.Error = error;
            session.Dispose();
        }

        private static void OnAccept(this WebSocketOuterComponent self, long channelId, IPEndPoint ipEndPoint)
        {
            Session session = self.AddChildWithId<Session, AService>(channelId, self.AService);
            session.RemoteAddress = ipEndPoint;
            session.AddComponent<SessionAcceptTimeoutComponent>();
            session.AddComponent<SessionIdleCheckerComponent>();
            session.AddComponent<MessageStatisticsComponent>();
        }

        private static void OnRead(this WebSocketOuterComponent self, long channelId, MemoryBuffer memoryBuffer)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }

            session.LastRecvTime = self.GetSingleton<TimeInfo>().ClientNow();
            (FiberInstanceId _, object message) = MessageSerializeHelper.ToMessage(self.AService, memoryBuffer);
            self.AService.Recycle(memoryBuffer);

            ushort opcode = OpcodeType.Instance.GetOpcode(message.GetType());
            if (opcode is > 20000 or < 10000)
            {
                Log.Error($"client message must in (10000, 20000), opcode: {opcode}");
                return;
            }

            LogMsg.Instance.Recv(self.Fiber(), message);
            EventSystem.Instance.Invoke(self.IScene.SceneType, new NetComponentOnRead() { Session = session, Message = message });
        }
    }
}
