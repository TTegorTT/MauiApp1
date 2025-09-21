using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MauiApp1.Model;

namespace MauiApp1.P2P;

public class P2PService
{
    private UdpClient Client { get; set; }
    private string Id { get; }
    private const int DiscoveryPort = 12345;
    private bool IsRunning { get; set; }

    // public event Action<PeerInfo> PeerDiscovered;
    public event Action<SpaceObject>? ThreatReceived;
    public event Action<string>? LogMessage;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public P2PService()
    {
        Id = GeneratePeerId();

        Client = new UdpClient(DiscoveryPort);
        Client.EnableBroadcast = true;
        Client.MulticastLoopback = true;
    }

    public void StartService()
    {
        if (IsRunning) return;

        try
        {
            IsRunning = true;

            _ = Task.Run(ListenForMessages);

            LogMessage?.Invoke("P2P служба запущена");
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка запуска: {ex.Message}");
        }
    }

    public void StopService()
    {
        IsRunning = false;
        Client.Close();
        LogMessage?.Invoke("P2P служба остановлена");
    }

    private async Task ListenForMessages()
    {
        while (IsRunning) await ReceiveMessageAsync();
    }

    private async Task ReceiveMessageAsync()
    {
        try
        {
            var result = await Client.ReceiveAsync();
            var messageJson = Encoding.UTF8.GetString(result.Buffer);

            LogMessage?.Invoke($"Получено сообщение: {messageJson}");

            var message = JsonSerializer.Deserialize<PeerMessage>(messageJson, _jsonOptions);

            if (message != null) ProcessMessage(message, result.RemoteEndPoint);
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка приема: {ex.Message}");
        }
    }

    public async Task ShareThreat(SpaceObject threat)
    {
        try
        {
            var threatData = new ThreatData
            {
                Threat = threat,
                SourcePeer = Id,
                Hops = 1,
                TTL = 5
            };

            var message = new PeerMessage
            {
                Type = "threat",
                PeerId = Id,
                Timestamp = DateTime.Now,
                Data = threatData
            };

            var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(messageJson);

            LogMessage?.Invoke($"Отправка угрозы: {threat.Coordinates}");

            // Также broadcast для обнаружения новых пиров
            var broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
            await Client.SendAsync(bytes, bytes.Length, broadcastEndPoint);
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка отправки угрозы: {ex.Message}");
        }
    }

    private void ProcessMessage(PeerMessage message, IPEndPoint remoteEndPoint)
    {
        try
        {
            switch (message.Type)
            {
                case "threat":
                    ProcessThreatMessage(message);
                    break;
            }
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    private void ProcessThreatMessage(PeerMessage message)
    {
        try
        {
            var threatDataJson = message.Data.ToString();
            var threatData = JsonSerializer.Deserialize<ThreatData>(threatDataJson, _jsonOptions);

            // Check TTL and avoid loops
            if (threatData.TTL <= 0 || threatData.SourcePeer == Id) return;

            // Decrease TTL and increase hops
            threatData.TTL--;
            threatData.Hops++;

            LogMessage?.Invoke(
                $"Получена угроза от {threatData.SourcePeer} (hops: {threatData.Hops}, TTL: {threatData.TTL})");

            // Add to local storage
            ThreatReceived?.Invoke(threatData.Threat);

            // Forward to other peers (if TTL still allows)
            if (threatData.TTL > 0)
            {
                _ = ShareThreat(threatData.Threat);
            }
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка обработки угрозы: {ex.Message}");
        }
    }

    private string GeneratePeerId()
    {
        return $"{DeviceInfo.Name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
    }

    private IPEndPoint ParseEndPoint(string endpoint)
    {
        try
        {
            var parts = endpoint.Split(':');
            if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var ipAddress))
            {
                return new IPEndPoint(ipAddress, int.Parse(parts[1]));
            }
        }
        catch
        {
        }

        return new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
    }
}