using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ET.Network.Tests;

[TestClass]
public class WebSocketServiceTests
{
    [TestMethod]
    public async Task PlainWebSocket_ExchangesLengthPrefixedBinaryFrame()
    {
        WebSocketServerConfig config = new()
        {
            Enable = true,
            Path = "/ws",
            UseTls = false,
        };
        await AssertRoundTrip(config, "ws");
    }

    [TestMethod]
    public async Task SecureWebSocket_ExchangesLengthPrefixedBinaryFrame()
    {
        string certificatePath = Path.Combine(Path.GetTempPath(), $"et-wss-{Guid.NewGuid():N}.pfx");
        const string password = "test-password";
        CreateCertificate(certificatePath, password);
        try
        {
            WebSocketServerConfig config = new()
            {
                Enable = true,
                Path = "/ws",
                UseTls = true,
                CertificatePath = certificatePath,
                CertificatePassword = password,
            };
            await AssertRoundTrip(config, "wss");
        }
        finally
        {
            File.Delete(certificatePath);
        }
    }

    private static async Task AssertRoundTrip(WebSocketServerConfig config, string scheme)
    {
        using WService service = new(new IPEndPoint(IPAddress.Loopback, 0), ServiceType.Outer, config);
        MemoryBuffer received = null;
        long acceptedId = 0;
        service.AcceptCallback = (id, _) => acceptedId = id;
        service.ReadCallback = (_, buffer) => received = buffer;

        using ClientWebSocket client = new();
        if (config.UseTls)
        {
            client.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        }

        Uri uri = new($"{scheme}://127.0.0.1:{service.GetBindPoint().Port}{config.Path}");
        await client.ConnectAsync(uri, CancellationToken.None);
        PumpUntil(service, () => service.ChannelCount > 0);

        byte[] body = [9, 8, 7, 6];
        byte[] packet = new byte[PacketParser.OuterPacketSizeLength + body.Length];
        BitConverter.GetBytes((ushort)body.Length).CopyTo(packet, 0);
        body.CopyTo(packet, PacketParser.OuterPacketSizeLength);
        await client.SendAsync(packet, WebSocketMessageType.Binary, true, CancellationToken.None);
        PumpUntil(service, () => received != null);

        Assert.IsNotNull(received);
        byte[] actual = new byte[body.Length];
        received.ReadExactly(actual, 0, actual.Length);
        CollectionAssert.AreEqual(body, actual);

        MemoryBuffer response = service.Fetch(body.Length);
        response.Write(body, 0, body.Length);
        response.Seek(0, SeekOrigin.Begin);
        service.Send(acceptedId, response);

        byte[] echoed = new byte[packet.Length];
        WebSocketReceiveResult result = await client.ReceiveAsync(echoed, CancellationToken.None);
        Assert.AreEqual(packet.Length, result.Count);
        CollectionAssert.AreEqual(packet, echoed);
    }

    private static void PumpUntil(WService service, Func<bool> condition)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            service.Update();
            Thread.Sleep(10);
        }

        service.Update();
        Assert.IsTrue(condition());
    }

    private static void CreateCertificate(string path, string password)
    {
        using RSA rsa = RSA.Create(2048);
        CertificateRequest request = new("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        SubjectAlternativeNameBuilder alternativeName = new();
        alternativeName.AddDnsName("localhost");
        alternativeName.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(alternativeName.Build());
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
        using X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(2));
        File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, password));
    }
}
