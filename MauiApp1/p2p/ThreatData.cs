using System.Text.Json.Serialization;
using MauiApp1.Model;

namespace MauiApp1.P2P;

public class ThreatData
{
    public required SpaceObject Threat { get; set; }
    public required string SourcePeer { get; set; }
    public required int Hops { get; set; } = 1;
    public required int TTL { get; set; } = 5;
}