using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Model;
using MauiApp1.P2P;

namespace MauiApp1;

public partial class P2pView : ContentView
{
    private Peer Peer { get; }
    private bool IsActive { get; set; }

    public event Action<SpaceObject>? SpaceObjectReceived;

    public P2pView()
    {
        InitializeComponent();

        Peer = new Peer();
        Peer.SpaceObjectReceived += OnSpaceObjectReceived;
        Peer.LogMessage += OnLogMessage;

        Start();
    }

    public void Start()
    {
        if (IsActive) return;

        try
        {
            Peer.Start();
            Update(true);
            Console.WriteLine("P2P служба запущена автоматически");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска P2P: {ex.Message}");
            Update(false);
        }
    }

    private void Update(bool isActive)
    {
        MainThread.BeginInvokeOnMainThread(() => p2pStatusIcon.Text = isActive ? "🟢" : "🔴");
    }

    private void OnSpaceObjectReceived(SpaceObject threat)
    {
        MainThread.BeginInvokeOnMainThread(() => SpaceObjectReceived?.Invoke(threat));
    }

    private void OnLogMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() => Console.WriteLine($"P2P: {message}"));
    }

    private void IconTapped(object sender, TappedEventArgs e)
    {
        IsActive = !IsActive;

        if (IsActive)
            Peer.Start();
        else
            Peer.Stop();

        Update(IsActive);
    }

    public void AddRandomObject()
    {
        var newObject = ObjectGenerator.GenerateRandomObject();
        OnSpaceObjectReceived(newObject);

        if (newObject.IsThreat) _ = Peer.ShareThreat(newObject);
    }

    private void Stop()
    {
        if (!IsActive) return;

        try
        {
            Peer.Stop();
            IsActive = false;

            Update(false);

            Console.WriteLine("P2P служба остановлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка остановки P2P: {ex.Message}");
        }
    }
}