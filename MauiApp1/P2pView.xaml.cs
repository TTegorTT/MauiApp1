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
    private P2PService _p2pService;

    private bool IsP2PEnabled { get; set; } = true;

    private bool IsActive { get; set; }
    // private const int P2PIntervalMinutes = 2; // Интервал работы P2P в минутах
    // private const int P2PWorkDurationMinutes = 1; // Длительность работы P2P в минутах

    // private System.Timers.Timer _p2pControlTimer;
    // private System.Timers.Timer autoAddTimer;
    // private bool isAutoAddEnabled = false;

    public event Action<SpaceObject>? SpaceObjectReceived;

    public P2pView()
    {
        InitializeComponent();

        _p2pService = new P2PService();
        // _p2pService.PeerDiscovered += OnPeerDiscovered;
        _p2pService.ThreatReceived += OnThreatReceived;
        _p2pService.LogMessage += OnP2PLogMessage;

        // Автоматически запускаем P2P
        Start();
    }

    public void Start()
    {
        if (IsActive) return;

        try
        {
            _p2pService.StartService();
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (isActive)
            {
                p2pStatusIcon.Text = "🟢";
                p2pStatusIcon.TextColor = Colors.Green;
            }
            else
            {
                p2pStatusIcon.Text = "🔴";
                p2pStatusIcon.TextColor = Colors.Red;
            }
        });
    }

    private void OnPeerDiscovered(PeerInfo peer)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Console.WriteLine($"Обнаружен пир: {peer.DeviceName}");
            // Можно обновлять счетчик пиров если нужно
        });
    }

    private void OnThreatReceived(SpaceObject threat)
    {
        MainThread.BeginInvokeOnMainThread(() => SpaceObjectReceived?.Invoke(threat));
    }

    private void OnP2PLogMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Console.WriteLine($"P2P: {message}");
            // Можно выводить в интерфейс если нужно
        });
    }

    private void IconTapped(object sender, TappedEventArgs e)
    {
        if (IsP2PEnabled)
        {
            _p2pService.StopService();
            IsP2PEnabled = false;
            Update(false);
        }
        else
        {
            _p2pService.StartService();
            IsP2PEnabled = true;
            Update(true);
        }
    }

    public void AddRandomObject()
    {
        var newObject = ObjectGenerator.GenerateRandomObject();
        OnThreatReceived(newObject);

        if (newObject.IsThreat)
        {
            // Автоматически отправляем угрозу через P2P
            _ = _p2pService.ShareThreat(newObject);
        }
    }

    // protected override void OnDisappearing()
    // {
    // base.OnDisappearing();
    // StopAutoAdd();
    // _p2pService.StopService();
    // }

    // private void StartTimer()
    // {
    //     _p2pControlTimer = new System.Timers.Timer(TimeSpan.FromMinutes(P2PIntervalMinutes).TotalMilliseconds);
    //     _p2pControlTimer.Elapsed += async (s, e) => await ControlP2PService();
    //     _p2pControlTimer.AutoReset = true;
    //     _p2pControlTimer.Start();
    // 
    //     // Запускаем P2P сразу при старте
    //     _ = Task.Run(async () => await ControlP2PService());
    // }

    // private async Task ControlP2PService()
    // {
    //     try
    //     {
    //         // Включаем P2P
    //         await StartP2P();
    // 
    //         // Ждем duration минут
    //         await Task.Delay(TimeSpan.FromMinutes(P2PWorkDurationMinutes));
    // 
    //         // Выключаем P2P
    //         StopP2P();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Ошибка управления P2P: {ex.Message}");
    //     }
    // }

    private void StopP2P()
    {
        if (!IsActive) return;

        try
        {
            _p2pService.StopService();
            IsActive = false;

            Update(false);

            Console.WriteLine("P2P служба остановлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка остановки P2P: {ex.Message}");
        }
    }

    // private async void OnShareThreatsClicked(object sender, EventArgs e)
    // {
    //     var threats = Objects.Where(o => o.IsThreat).ToList();
    //     if (!threats.Any())
    //     {
    //         await DisplayAlert("Нет угроз", "Нет угроз для обмена", "OK");
    //         return;
    //     }
    // 
    //     foreach (var threat in threats)
    //     {
    //         await _p2pService.ShareThreat(threat);
    //     }
    // 
    //     await DisplayAlert("Обмен", $"Отправлено {threats.Count} угроз", "OK");
    // }

    // private void OnAddObjectClicked(object sender, EventArgs e)
    // {
    // AddRandomObject();
    // }

    // private void OnAutoAddClicked(object sender, EventArgs e)
    // {
    //     if (isAutoAddEnabled)
    //     {
    //         StopAutoAdd();
    //         autoAddButton.Text = "🔁 Авто-добавление";
    //         autoAddButton.BackgroundColor = Color.FromArgb("#2980B9");
    //     }
    //     else
    //     {
    //         StartAutoAdd();
    //         autoAddButton.Text = "⏹️ Остановить";
    //         autoAddButton.BackgroundColor = Color.FromArgb("#E74C3C");
    //     }
// }

// private void StartAutoAdd()
// {
//     autoAddTimer = new System.Timers.Timer(5000);
//     autoAddTimer.Elapsed += (s, e) =>
//         MainThread.BeginInvokeOnMainThread(AddRandomObject);
//     autoAddTimer.AutoReset = true;
//     autoAddTimer.Start();
//     isAutoAddEnabled = true;
// }
// 
// private void StopAutoAdd()
// {
//     autoAddTimer?.Stop();
//     autoAddTimer?.Dispose();
//     isAutoAddEnabled = false;
// }
}