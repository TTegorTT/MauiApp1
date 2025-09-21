using System.Text.Json.Serialization;
using MauiApp1.Model;

namespace MauiApp1.P2P;

public class ThreatData
{
    [JsonPropertyName("threat")]
    public SpaceObject Threat { get; set; }

    [JsonPropertyName("sourcePeer")]
    public string SourcePeer { get; set; }

    [JsonPropertyName("hops")]
    public int Hops { get; set; } = 1;

    [JsonPropertyName("ttl")]
    public int TTL { get; set; } = 5; // Time to live
}