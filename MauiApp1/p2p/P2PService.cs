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
    private List<PeerInfo> knownPeers { get; } = [];
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
            _ = Task.Run(SendDiscoveryBeacons);

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

    private async Task SendDiscoveryBeacons()
    {
        while (IsRunning)
        {
            try
            {
                var discoveryData = new DiscoveryData
                {
                    DeviceName = DeviceInfo.Name,
                    AppVersion = AppInfo.VersionString,
                    Capabilities = new List<string>
                    {
                        "threat_sharing"
                    }
                };

                var message = new PeerMessage
                {
                    Type = "discovery",
                    PeerId = Id,
                    Timestamp = DateTime.Now,
                    Data = discoveryData
                };

                var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
                var bytes = Encoding.UTF8.GetBytes(messageJson);

                // Broadcast discovery message
                var broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
                await Client.SendAsync(bytes, bytes.Length, broadcastEndPoint);

                // Также отправляем на локальный адрес для тестирования
                var localEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.255"), DiscoveryPort);
                await Client.SendAsync(bytes, bytes.Length, localEndPoint);

                LogMessage?.Invoke("Отправлен discovery beacon");

                await Task.Delay(3000); // Send every 3 seconds
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Ошибка отправки: {ex.Message}");
            }
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

            // Send to all known peers
            foreach (var peer in knownPeers)
                try
                {
                    var endPoint = ParseEndPoint(peer.Endpoint);
                    await Client.SendAsync(bytes, bytes.Length, endPoint);
                    LogMessage?.Invoke($"Угроза отправлена пиру: {peer.DeviceName}");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Ошибка отправки пиру {peer.DeviceName}: {ex.Message}");
                }

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
                case "discovery":
                    ProcessDiscoveryMessage(message, remoteEndPoint);
                    break;
                case "threat":
                    ProcessThreatMessage(message);
                    break;
                case "ack":
                    ProcessAckMessage(message);
                    break;
            }
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    private void ProcessDiscoveryMessage(PeerMessage message, IPEndPoint remoteEndPoint)
    {
        if (message.PeerId == Id) return;

        try
        {
            var discoveryData = JsonSerializer.Deserialize<DiscoveryData>(message.Data.ToString(), _jsonOptions);
            var peerEndpoint = $"{remoteEndPoint.Address}:{remoteEndPoint.Port}";

            var existingPeer = knownPeers.FirstOrDefault(p => p.Id == message.PeerId);
            if (existingPeer == null)
            {
                var newPeer = new PeerInfo
                {
                    Id = message.PeerId,
                    DeviceName = discoveryData.DeviceName,
                    LastSeen = DateTime.Now,
                    Endpoint = peerEndpoint
                };

                knownPeers.Add(newPeer);
                // PeerDiscovered?.Invoke(newPeer);
                LogMessage?.Invoke($"Обнаружен пир: {discoveryData.DeviceName}");
            }
            else
            {
                existingPeer.LastSeen = DateTime.Now;
                existingPeer.Endpoint = peerEndpoint;
            }
        }
        catch (Exception ex)
        {
            LogMessage?.Invoke($"Ошибка обработки discovery: {ex.Message}");
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

    private void ProcessAckMessage(PeerMessage message)
    {
        // Handle acknowledgements if needed
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
            // В случае ошибки используем broadcast
        }

        return new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
    }

    // public List<PeerInfo> GetKnownPeers()
    // => knownPeers;

    public void CleanupOldPeers()
    {
        knownPeers.RemoveAll(p => (DateTime.Now - p.LastSeen).TotalMinutes > 5);
    }
}