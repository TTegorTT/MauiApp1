using MauiApp1.Model;

namespace MauiApp1.P2P;

public class PeerInfo
{
    public string Id { get; set; }
    public string DeviceName { get; set; }
    public DateTime LastSeen { get; set; }
    public string Endpoint { get; set; }
    public List<SpaceObject> SharedThreats { get; set; } = new();
}