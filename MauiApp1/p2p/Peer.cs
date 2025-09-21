using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MauiApp1.Model;

namespace MauiApp1.P2P;

public class Peer
{
    private UdpClient Client { get; set; }
    private string Id { get; }
    private const int DiscoveryPort = 12345;
    private bool IsRunning { get; set; }

    public event Action<SpaceObject>? SpaceObjectReceived;
    public event Action<string>? LogMessage;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public Peer()
    {
        Id = GeneratePeerId();

        Client = new UdpClient(DiscoveryPort);
        Client.EnableBroadcast = true;
        Client.MulticastLoopback = true;
    }

    public void Start()
    {
        if (IsRunning) return;

        try
        {
            IsRunning = true;

            ListenThread = Task.Run(ListenForMessages);

            LogMessage?.Invoke("P2P служба запущена");
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка запуска: {ex.Message}");
        }
    }

    private Task? ListenThread { get; set; }

    public void Stop()
    {
        IsRunning = false;
        ListenThread?.Wait();

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

            if (message != null) await ProcessMessage(message);
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
                PeerId = Id,
                Data = threatData
            };

            var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(messageJson);

            LogMessage?.Invoke($"Отправка угрозы: {threat.Coordinates}");

            var broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
            await Client.SendAsync(bytes, bytes.Length, broadcastEndPoint);
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка отправки угрозы: {ex.Message}");
        }
    }

    private async Task ProcessMessage(PeerMessage message)
    {
        try
        {
            await ProcessThreatMessage(message);
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    private async Task ProcessThreatMessage(PeerMessage message)
    {
        try
        {
            var threatDataJson = message.Data.ToString();
            Debug.Assert(threatDataJson is not null);

            var threatData = JsonSerializer.Deserialize<ThreatData>(threatDataJson, _jsonOptions);
            Debug.Assert(threatData is not null);

            if (threatData.TTL <= 0 || threatData.SourcePeer == Id) return;

            threatData.TTL--;
            threatData.Hops++;

            LogMessage?.Invoke(
                $"Получена угроза от {threatData.SourcePeer} (hops: {threatData.Hops}, TTL: {threatData.TTL})");

            SpaceObjectReceived?.Invoke(threatData.Threat);

            if (threatData.TTL > 0) await ShareThreat(threatData.Threat);
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
}