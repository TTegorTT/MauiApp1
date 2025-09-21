using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using MauiApp1.Converters;
using MauiApp1.Model;
using MauiApp1.P2P;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private P2PService _p2pService;
    private bool _isP2PEnabled = true;
    private bool _isP2PActive = false;
    private const int P2PIntervalMinutes = 2; // Интервал работы P2P в минутах
    private const int P2PWorkDurationMinutes = 1; // Длительность работы P2P в минутах
    private System.Timers.Timer _p2pControlTimer;
    public ObservableCollection<SpaceObject> Objects { get; set; } = new ObservableCollection<SpaceObject>();

    private System.Timers.Timer autoAddTimer;
    private bool isAutoAddEnabled = false;
    private int threatCount = 0;

    public MainPage()
    {
        InitializeComponent();
        objectsCollectionView.ItemsSource = Objects;

        // _p2pService = new P2PService();
        // _p2pService.PeerDiscovered += OnPeerDiscovered;
        // _p2pService.ThreatReceived += OnThreatReceived;
        // _p2pService.LogMessage += OnP2PLogMessage;

        // Автоматически запускаем P2P
        // _ = StartP2PService();

        // Регистрируем конвертеры
        Resources.Add("ThreatToColorConverter", new ThreatToColorConverter());
        Resources.Add("TypeToEmojiConverter", new TypeToEmojiConverter());
        Resources.Add("TypeToStringConverter", new TypeToStringConverter());
        Resources.Add("ThreatToEmojiConverter", new ThreatToEmojiConverter());
        Resources.Add("ThreatToStringConverter", new ThreatToStringConverter());
    }

    private async Task StartP2PService()
    {
        try
        {
            await _p2pService.StartService();
            UpdateP2PStatus(true);
            Console.WriteLine("P2P служба запущена автоматически");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска P2P: {ex.Message}");
            UpdateP2PStatus(false);
        }
    }

    private void UpdateP2PStatus(bool isActive)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (isActive)
            {
                p2pStatusIcon.Text = "🟢"; // Зеленый - активно
                p2pStatusIcon.TextColor = Colors.Green;
            }
            else
            {
                p2pStatusIcon.Text = "🔴"; // Красный - неактивно
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Проверяем, нет ли уже такой угрозы
            if (!Objects.Any(o => o.Coordinates == threat.Coordinates &&
                                  Math.Abs((o.ArrivalTime - threat.ArrivalTime).TotalSeconds) < 1))
            {
                Objects.Add(threat);
                UpdateStatistics();

                Console.WriteLine($"Получена угроза от P2P: {threat.Coordinates}");

                // Прокрутка к новому объекту
                objectsCollectionView.ScrollTo(Objects.Count - 1, position: ScrollToPosition.End);
            }
        });
    }

    private void OnP2PLogMessage(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Console.WriteLine($"P2P: {message}");
            // Можно выводить в интерфейс если нужно
        });
    }

    // Автоматически отправляем новые угрозы
    private void AddRandomObject()
    {
        var newObject = ObjectGenerator.GenerateRandomObject();
        Objects.Add(newObject);

        if (newObject.IsThreat)
        {
            threatCount++;
            UpdateStatistics();

            // Автоматически отправляем угрозу через P2P
            _ = _p2pService.ShareThreat(newObject);
        }

        // Прокрутка к новому объекту
        objectsCollectionView.ScrollTo(Objects.Count - 1, position: ScrollToPosition.End);
    }

    private void UpdateStatistics()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            threatCount = Objects.Count(o => o.IsThreat);
            // Обновляем статистику если нужно
        });
    }

    // Обработчик для ручного управления (если нужно)
    private async void OnP2PStatusIconTapped(object sender, TappedEventArgs e)
    {
        if (_isP2PEnabled)
        {
            _p2pService.StopService();
            _isP2PEnabled = false;
            UpdateP2PStatus(false);
        }
        else
        {
            await _p2pService.StartService();
            _isP2PEnabled = true;
            UpdateP2PStatus(true);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopAutoAdd();
        _p2pService.StopService();
    }

    private void StartP2PControlTimer()
    {
        _p2pControlTimer = new System.Timers.Timer(TimeSpan.FromMinutes(P2PIntervalMinutes).TotalMilliseconds);
        _p2pControlTimer.Elapsed += async (s, e) => await ControlP2PService();
        _p2pControlTimer.AutoReset = true;
        _p2pControlTimer.Start();

        // Запускаем P2P сразу при старте
        _ = Task.Run(async () => await ControlP2PService());
    }

    private async Task ControlP2PService()
    {
        try
        {
            // Включаем P2P
            await StartP2P();

            // Ждем duration минут
            await Task.Delay(TimeSpan.FromMinutes(P2PWorkDurationMinutes));

            // Выключаем P2P
            StopP2P();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка управления P2P: {ex.Message}");
        }
    }

    private async Task StartP2P()
    {
        if (_isP2PActive) return;

        try
        {
            await _p2pService.StartService();
            _isP2PActive = true;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                p2pStatusIcon.Text = "🟢"; // Зеленый - активно
                p2pStatusIcon.TextColor = Colors.Green;
            });

            Console.WriteLine("P2P служба запущена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска P2P: {ex.Message}");
        }
    }

    private void StopP2P()
    {
        if (!_isP2PActive) return;

        try
        {
            _p2pService.StopService();
            _isP2PActive = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                p2pStatusIcon.Text = "🔴"; // Красный - неактивно
                p2pStatusIcon.TextColor = Colors.Red;
            });

            Console.WriteLine("P2P служба остановлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка остановки P2P: {ex.Message}");
        }
    }

    private async void OnP2PToggleClicked(object sender, EventArgs e)
    {
        if (_isP2PEnabled)
        {
            _p2pService.StopService();
            p2pStatusLabel.Text = "P2P: Выкл";
            p2pToggleButton.Text = "Включить P2P";
            p2pToggleButton.BackgroundColor = Color.FromArgb("#2196F3");
        }
        else
        {
            await _p2pService.StartService();
            p2pStatusLabel.Text = "P2P: Вкл";
            p2pToggleButton.Text = "Выключить P2P";
            p2pToggleButton.BackgroundColor = Color.FromArgb("#F44336");
        }

        _isP2PEnabled = !_isP2PEnabled;
    }

    private async void OnShareThreatsClicked(object sender, EventArgs e)
    {
        var threats = Objects.Where(o => o.IsThreat).ToList();
        if (!threats.Any())
        {
            await DisplayAlert("Нет угроз", "Нет угроз для обмена", "OK");
            return;
        }

        foreach (var threat in threats)
        {
            await _p2pService.ShareThreat(threat);
        }

        await DisplayAlert("Обмен", $"Отправлено {threats.Count} угроз", "OK");
    }

    private void OnAddObjectClicked(object sender, EventArgs e)
    {
        AddRandomObject();
    }

    private void OnAutoAddClicked(object sender, EventArgs e)
    {
        if (isAutoAddEnabled)
        {
            StopAutoAdd();
            autoAddButton.Text = "🔁 Авто-добавление";
            autoAddButton.BackgroundColor = Color.FromArgb("#2980B9");
        }
        else
        {
            StartAutoAdd();
            autoAddButton.Text = "⏹️ Остановить";
            autoAddButton.BackgroundColor = Color.FromArgb("#E74C3C");
        }
    }

    private void StartAutoAdd()
    {
        autoAddTimer = new System.Timers.Timer(5000);
        autoAddTimer.Elapsed += (s, e) =>
            MainThread.BeginInvokeOnMainThread(AddRandomObject);
        autoAddTimer.AutoReset = true;
        autoAddTimer.Start();
        isAutoAddEnabled = true;
    }

    private void StopAutoAdd()
    {
        autoAddTimer?.Stop();
        autoAddTimer?.Dispose();
        isAutoAddEnabled = false;
    }

    private async void OnObjectDoubleTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame &&
            frame.BindingContext is SpaceObject spaceObject)
        {
            await ShowObjectDetails(spaceObject);
        }
    }

    private async Task ShowObjectDetails(SpaceObject obj)
    {
        var threatEmoji = obj.Threat switch
        {
            ThreatLevel.CriticalThreat => "💀",
            ThreatLevel.HighThreat => "🔥",
            ThreatLevel.MediumThreat => "⚠️",
            ThreatLevel.LowThreat => "🔶",
            _ => "✅"
        };

        var details =
            $"""
             {threatEmoji} {ObjectGenerator.GetThreatLevelName(obj.Threat).ToUpper()}

             🛰️ Тип: {ObjectGenerator.GetObjectTypeName(obj.Type)}
             🕒 Время прибытия: {obj.ArrivalTime:0:dd.MM.yyyy HH:mm:ss}
             📍 Координаты: {obj.Coordinates}

             📏 Расстояние: {obj.Distance:F0} м
             🚀 Скорость: {obj.Speed:F0} км/ч
             🔷 Форма: {ObjectGenerator.GetShapeName(obj.Shape)}
             📊 Размер: {obj.Size:F0} м
             ⚖️ Плотность: {obj.Density:F0} кг/м³
             🌡️ Температура: {obj.Temperature:F0} °C
             🧲 Магнитное поле: {obj.MagneticField:F6} Тл

             📡 ЭМИ спектр:
                • Частота: {obj.EmissionFrequency:E2} Гц
                • Длина волны: {obj.Wavelength:F0} м

             ☢️ Ионизирующее излучение: 
                {ObjectGenerator.GetRadiationName(obj.Radiation)}

             {(obj.IsThreat ? "🚨 ОБЪЕКТ ПРЕДСТАВЛЯЕТ УГРОЗУ!" : "✅ Объект безопасен")}
             """;

        await DisplayAlert("Детали объекта", details, "Закрыть");
    }
}