using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ET
{
    public sealed class WService: AService
    {
        private readonly Dictionary<long, WChannel> idChannels = new();
        private readonly ConcurrentQueue<Action> mainThreadActions = new();
        private readonly WebSocketServerConfig config;
        private readonly CancellationTokenSource cancellation = new();
        private WebApplication app;
        private long idGenerator;

        public WService(IPEndPoint ipEndPoint, ServiceType serviceType, WebSocketServerConfig config)
        {
            this.ServiceType = serviceType;
            this.config = config ?? new WebSocketServerConfig();
            this.StartAccept(ipEndPoint);
        }

        public WService(ServiceType serviceType, WebSocketServerConfig config)
        {
            this.ServiceType = serviceType;
            this.config = config ?? new WebSocketServerConfig();
        }

        public long NewId()
        {
            return Interlocked.Increment(ref this.idGenerator);
        }

        public int ChannelCount => this.idChannels.Count;

        public WChannel Get(long id)
        {
            this.idChannels.TryGetValue(id, out WChannel channel);
            return channel;
        }

        public void Add(WChannel channel)
        {
            this.idChannels[channel.Id] = channel;
        }

        public override IPEndPoint GetBindPoint()
        {
            return this.BindPoint;
        }

        public IPEndPoint BindPoint { get; private set; }

        public override void Create(long id, IPEndPoint ipEndPoint)
        {
            if (this.idChannels.ContainsKey(id))
            {
                return;
            }

            WChannel channel = new(id, ipEndPoint, this, this.config);
            this.AcceptCallback?.Invoke(id, ipEndPoint);
            _ = this.ReceiveLoop(channel);
        }

        public override void Send(long channelId, MemoryBuffer memoryBuffer)
        {
            WChannel channel = this.Get(channelId);
            if (channel == null)
            {
                this.ErrorCallback?.Invoke(channelId, ErrorCore.ERR_WebsocketSendError);
                return;
            }

            try
            {
                channel.Send(memoryBuffer);
            }
            catch (Exception e)
            {
                Log.Error(e);
                channel.MarkError(ErrorCore.ERR_WebsocketSendError);
                this.ErrorCallback?.Invoke(channelId, ErrorCore.ERR_WebsocketSendError);
                channel.Dispose();
            }
        }

        public override void Remove(long id, int error = 0)
        {
            if (this.idChannels.TryGetValue(id, out WChannel channel))
            {
                channel.MarkError(error);
                channel.Dispose();
            }
        }

        public void RemoveChannel(long id, int error)
        {
            this.idChannels.Remove(id);
            if (error != 0)
            {
                this.ErrorCallback?.Invoke(id, error);
            }
        }

        public override void Update()
        {
            while (this.mainThreadActions.TryDequeue(out Action action))
            {
                action();
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed())
            {
                return;
            }

            base.Dispose();
            this.cancellation.Cancel();
            foreach (long id in new List<long>(this.idChannels.Keys))
            {
                if (this.idChannels.TryGetValue(id, out WChannel channel))
                {
                    channel.Dispose();
                }
            }

            if (this.app != null)
            {
                this.app.StopAsync().GetAwaiter().GetResult();
                this.app.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }

        private void StartAccept(IPEndPoint ipEndPoint)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = "ET.WebSocket",
                Args = Array.Empty<string>(),
            });
            builder.Logging.ClearProviders();
            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(ipEndPoint.Address, ipEndPoint.Port, listen =>
                {
                    if (!this.config.UseTls)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(this.config.CertificatePath))
                    {
                        throw new Exception("WSS 已启用，但没有配置证书路径");
                    }

                    listen.UseHttps(this.config.CertificatePath, this.config.CertificatePassword ?? "");
                });
            });

            WebApplication application = builder.Build();
            application.UseWebSockets();
            string path = string.IsNullOrEmpty(this.config.Path) ? "/ws" : this.config.Path;
            application.Map(path, async context =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                IPEndPoint remote = new(context.Connection.RemoteIpAddress ?? IPAddress.Loopback, context.Connection.RemotePort);
                long id = this.NewId();
                TaskCompletionSource<WChannel> created = new(TaskCreationOptions.RunContinuationsAsynchronously);
                this.mainThreadActions.Enqueue(() =>
                {
                    WChannel channel = new(id, socket, remote, this);
                    created.TrySetResult(channel);
                    this.AcceptCallback?.Invoke(id, remote);
                });
                WChannel accepted = await created.Task.WaitAsync(this.cancellation.Token);
                await this.ReceiveLoop(accepted);
            });

            application.StartAsync(this.cancellation.Token).GetAwaiter().GetResult();
            this.app = application;
            this.BindPoint = this.ReadBoundEndPoint(ipEndPoint);
        }

        private IPEndPoint ReadBoundEndPoint(IPEndPoint requested)
        {
            IServer server = this.app.Services.GetRequiredService<IServer>();
            IServerAddressesFeature feature = server.Features.Get<IServerAddressesFeature>();
            foreach (string address in feature.Addresses)
            {
                if (Uri.TryCreate(address, UriKind.Absolute, out Uri uri))
                {
                    return new IPEndPoint(requested.Address, uri.Port);
                }
            }

            return requested;
        }

        private async Task ReceiveLoop(WChannel channel)
        {
            byte[] buffer = new byte[8192];
            try
            {
                while (!channel.IsDisposed && !this.cancellation.IsCancellationRequested)
                {
                    using MemoryStream message = new();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await channel.ReceiveAsync(buffer, this.cancellation.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            this.mainThreadActions.Enqueue(() =>
                            {
                                channel.MarkError(ErrorCore.ERR_WebsocketPeerReset);
                                channel.Dispose();
                            });
                            return;
                        }

                        message.Write(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    byte[] payload = message.ToArray();
                    this.mainThreadActions.Enqueue(() =>
                    {
                        if (channel.IsDisposed)
                        {
                            return;
                        }

                        channel.Enqueue(payload);
                        channel.ParseIncoming();
                    });
                }
            }
            catch (Exception)
            {
                this.mainThreadActions.Enqueue(() =>
                {
                    if (channel.IsDisposed)
                    {
                        return;
                    }

                    channel.MarkError(ErrorCore.ERR_WebsocketRecvError);
                    channel.Dispose();
                });
            }
        }
    }
}
