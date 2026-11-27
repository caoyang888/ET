using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ET
{
    public class WebSocketTransport: IKcpTransport
    {
        private readonly WService service;
        private readonly DoubleMap<long, EndPoint> idEndpoints = new();
        private readonly Queue<(EndPoint, MemoryBuffer)> channelRecvDatas = new();
        private readonly Dictionary<long, long> readWriteTime = new();
        private readonly Queue<long> channelIds = new();

        public WebSocketTransport(IPEndPoint ipEndPoint, WebSocketServerConfig config)
        {
            this.service = new WService(ipEndPoint, ServiceType.Outer, config);
            this.service.AcceptCallback = this.OnAccept;
            this.service.ErrorCallback = this.OnError;
            this.service.ReadCallback = this.OnRead;
        }

        public WebSocketTransport(IPEndPoint ipEndPoint): this(ipEndPoint, WebSocketServerConfig.Load())
        {
        }

        private void OnAccept(long id, IPEndPoint ipEndPoint)
        {
            WChannel channel = this.service.Get(id);
            this.readWriteTime[id] = TimeInfo.Instance.ClientNow();
            this.channelIds.Enqueue(id);
            this.idEndpoints.Add(id, channel.RemoteAddress);
        }

        public void OnError(long id, int error)
        {
            Log.Warning($"IKcpTransport websocket error: {id} {error}");
            this.service.Remove(id, error);
            this.idEndpoints.RemoveByKey(id);
            this.readWriteTime.Remove(id);
        }

        private void OnRead(long id, MemoryBuffer memoryBuffer)
        {
            this.readWriteTime[id] = TimeInfo.Instance.ClientNow();
            WChannel channel = this.service.Get(id);
            this.channelRecvDatas.Enqueue((channel.RemoteAddress, memoryBuffer));
        }

        public void Send(byte[] bytes, int index, int length, EndPoint endPoint, ChannelType channelType)
        {
            long id = this.idEndpoints.GetKeyByValue(endPoint);
            if (id == 0)
            {
                if (channelType != ChannelType.Connect)
                {
                    return;
                }

                id = this.service.NewId();
                this.service.Create(id, (IPEndPoint)endPoint);
                this.idEndpoints.Add(id, endPoint);
                this.channelIds.Enqueue(id);
            }

            MemoryBuffer memoryBuffer = this.service.Fetch();
            memoryBuffer.Write(bytes, index, length);
            memoryBuffer.Seek(0, SeekOrigin.Begin);
            this.service.Send(id, memoryBuffer);
            this.readWriteTime[id] = TimeInfo.Instance.ClientNow();
        }

        public int Recv(byte[] buffer, ref EndPoint endPoint)
        {
            (EndPoint e, MemoryBuffer memoryBuffer) = this.channelRecvDatas.Dequeue();
            endPoint = e;
            int count = memoryBuffer.Read(buffer);
            this.service.Recycle(memoryBuffer);
            return count;
        }

        public IPEndPoint GetBindPoint()
        {
            return this.service.GetBindPoint();
        }

        public bool Available()
        {
            return this.channelRecvDatas.Count > 0;
        }

        public void Update()
        {
            long timeNow = TimeInfo.Instance.ClientNow();
            const int MaxCheckNum = 10;
            int n = this.channelIds.Count < MaxCheckNum ? this.channelIds.Count : MaxCheckNum;
            for (int i = 0; i < n; ++i)
            {
                long id = this.channelIds.Dequeue();
                if (!this.readWriteTime.TryGetValue(id, out long rwTime))
                {
                    continue;
                }

                if (timeNow - rwTime > 30 * 1000)
                {
                    this.OnError(id, ErrorCore.ERR_KcpReadWriteTimeout);
                    continue;
                }

                this.channelIds.Enqueue(id);
            }

            this.service.Update();
        }

        public void Dispose()
        {
            this.service?.Dispose();
        }
    }
}
