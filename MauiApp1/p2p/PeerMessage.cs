namespace MauiApp1.P2P;

public class PeerMessage
{
    public required string PeerId { get; set; }

    public required object Data { get; set; }
}