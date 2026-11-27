using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    public sealed class WChannel: AChannel
    {
        private readonly WService service;
        private readonly WebSocket socket;
        private readonly CircularBuffer recvBuffer = new();
        private readonly PacketParser parser;
        private readonly ConcurrentQueue<byte[]> incoming = new();
        private int error;

        public WChannel(long id, WebSocket socket, IPEndPoint remoteAddress, WService service)
        {
            this.service = service;
            this.socket = socket;
            this.Id = id;
            this.ChannelType = ChannelType.Accept;
            this.RemoteAddress = remoteAddress;
            this.parser = new PacketParser(this.recvBuffer, this.service);
            this.service.Add(this);
        }

        public WChannel(long id, IPEndPoint remoteAddress, WService service, WebSocketServerConfig config)
        {
            this.service = service;
            this.Id = id;
            this.ChannelType = ChannelType.Connect;
            this.RemoteAddress = remoteAddress;
            this.parser = new PacketParser(this.recvBuffer, this.service);
            ClientWebSocket client = new();
            if (config.UseTls)
            {
                client.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            }

            string scheme = config.UseTls ? "wss" : "ws";
            string path = string.IsNullOrEmpty(config.Path) ? "/ws" : config.Path;
            Uri uri = new($"{scheme}://{remoteAddress.Address}:{remoteAddress.Port}{path}");
            client.ConnectAsync(uri, CancellationToken.None).GetAwaiter().GetResult();
            this.socket = client;
            this.service.Add(this);
        }

        public Task<WebSocketReceiveResult> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken)
        {
            return this.socket.ReceiveAsync(buffer, cancellationToken);
        }

        public void Enqueue(byte[] data)
        {
            this.incoming.Enqueue(data);
        }

        public void ParseIncoming()
        {
            while (this.incoming.TryDequeue(out byte[] data))
            {
                this.recvBuffer.Write(data, 0, data.Length);
                while (this.parser.Parse(out MemoryBuffer memoryBuffer))
                {
                    this.service.ReadCallback?.Invoke(this.Id, memoryBuffer);
                }
            }
        }

        public void Send(MemoryBuffer stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("WChannel已经被Dispose, 不能发送消息");
            }

            int messageSize = (int)(stream.Length - stream.Position);
            int sizeLength = this.service.ServiceType == ServiceType.Inner ? PacketParser.InnerPacketSizeLength : PacketParser.OuterPacketSizeLength;
            byte[] packet = new byte[sizeLength + messageSize];
            if (sizeLength == PacketParser.OuterPacketSizeLength)
            {
                BitConverter.GetBytes((ushort)messageSize).CopyTo(packet, 0);
            }
            else
            {
                BitConverter.GetBytes(messageSize).CopyTo(packet, 0);
            }

            stream.Read(packet, sizeLength, messageSize);
            this.socket.SendAsync(packet, WebSocketMessageType.Binary, true, CancellationToken.None).GetAwaiter().GetResult();
            this.service.Recycle(stream);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            long id = this.Id;
            this.Id = 0;
            try
            {
                if (this.socket.State == WebSocketState.Open)
                {
                    this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "dispose", CancellationToken.None).GetAwaiter().GetResult();
                }
            }
            catch
            {
            }

            this.socket.Dispose();
            this.service.RemoveChannel(id, this.error);
        }

        public void MarkError(int errorCode)
        {
            this.error = errorCode;
            this.Error = errorCode;
        }
    }
}
