using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace ET
{
    public sealed class WebSocketServerConfig
    {
        public bool Enable { get; set; } = true;

        public string Path { get; set; } = "/ws";

        public int PortOffset { get; set; } = 1000;

        public bool UseTls { get; set; }

        public string CertificatePath { get; set; } = "";

        public string CertificatePassword { get; set; } = "";

        public string Scheme => this.UseTls ? "wss" : "ws";

        public IPEndPoint Shift(IPEndPoint outerAddress)
        {
            int port = outerAddress.Port + this.PortOffset;
            if (port is < 1 or > 65535)
            {
                throw new Exception($"WebSocket 端口越界: {outerAddress.Port} + {this.PortOffset}");
            }

            return new IPEndPoint(outerAddress.Address, port);
        }

        public static WebSocketServerConfig Load(string path = null)
        {
            path ??= System.IO.Path.Combine("Config", "websocket.json");
            if (!File.Exists(path))
            {
                return new WebSocketServerConfig();
            }

            string json = File.ReadAllText(path);
            WebSocketServerConfig config = JsonSerializer.Deserialize<WebSocketServerConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            return config ?? new WebSocketServerConfig();
        }
    }
}
