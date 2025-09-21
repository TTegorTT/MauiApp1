using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Model;
using MauiApp1.P2P;

namespace MauiApp1;

// Объект HASH

// Устройство 

public partial class P2pView : ContentView
{
    private P2PService _p2pService;
    private bool IsP2PEnabled { get; set; } = true;
    private bool IsActive { get; set; }

    public event Action<SpaceObject>? SpaceObjectReceived;

    public P2pView()
    {
        InitializeComponent();

        _p2pService = new P2PService();
        _p2pService.ThreatReceived += OnThreatReceived;
        _p2pService.LogMessage += OnP2PLogMessage;

        Start();
    }

    public void Start()
    {
        if (IsActive) return;

        try
        {
            _p2pService.Start();
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

    private void OnThreatReceived(SpaceObject threat)
    {
        MainThread.BeginInvokeOnMainThread(() => SpaceObjectReceived?.Invoke(threat));
    }

    private void OnP2PLogMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() => Console.WriteLine($"P2P: {message}"));
    }

    private void IconTapped(object sender, TappedEventArgs e)
    {
        IsP2PEnabled = !IsP2PEnabled;

        if (IsP2PEnabled)
            _p2pService.Start();
        else
            _p2pService.Stop();

        Update(IsP2PEnabled);
    }

    public void AddRandomObject()
    {
        var newObject = ObjectGenerator.GenerateRandomObject();
        OnThreatReceived(newObject);

        if (newObject.IsThreat) _ = _p2pService.ShareThreat(newObject);
    }

    private void StopP2P()
    {
        if (!IsActive) return;

        try
        {
            _p2pService.Stop();
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