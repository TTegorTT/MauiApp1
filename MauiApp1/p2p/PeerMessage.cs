using System.Text.Json.Serialization;

namespace MauiApp1.P2P;

public class PeerMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } // "discovery", "threat", "ack"

    [JsonPropertyName("peerId")]
    public string PeerId { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; }
}