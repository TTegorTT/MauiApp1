using System.Text.Json.Serialization;

namespace MauiApp1.P2P;

public class DiscoveryData
{
    [JsonPropertyName("deviceName")]
    public string DeviceName { get; set; }

    [JsonPropertyName("appVersion")]
    public string AppVersion { get; set; }

    [JsonPropertyName("capabilities")]
    public List<string> Capabilities { get; set; } = new();
}