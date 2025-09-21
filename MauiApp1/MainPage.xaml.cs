using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using MauiApp1.Converters;
using MauiApp1.Model;
using MauiApp1.P2P;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public ObservableCollection<SpaceObject> Objects { get; set; } = new ObservableCollection<SpaceObject>();

    public MainPage()
    {
        InitializeComponent();
        objectsCollectionView.ItemsSource = Objects;

        // Регистрируем конвертеры
        Resources.Add("ThreatToColorConverter", new ThreatToColorConverter());
        Resources.Add("TypeToEmojiConverter", new TypeToEmojiConverter());
        Resources.Add("TypeToStringConverter", new TypeToStringConverter());
        Resources.Add("ThreatToEmojiConverter", new ThreatToEmojiConverter());
        Resources.Add("ThreatToStringConverter", new ThreatToStringConverter());
    }

    void a(SpaceObject spaceObject)
    {
        // Проверяем, нет ли уже такой угрозы
        if (Objects.Any(o => o.Coordinates == spaceObject.Coordinates &&
                             Math.Abs((o.ArrivalTime - spaceObject.ArrivalTime).TotalSeconds) < 1))
            return;
        Objects.Add(spaceObject);

        Console.WriteLine($"Получена угроза от P2P: {spaceObject.Coordinates}");
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

    private async void SpaceObjectView_OnDoubleTapped(SpaceObject obj)
    {
        await ShowObjectDetails(obj);
    }

    private void OnAddObjectClicked(object? sender, EventArgs e)
    {
        P2p.AddRandomObject();
    }

    private void OnAutoAddClicked(object? sender, EventArgs e)
    {
    }

    private void P2pView_OnSpaceObjectReceived(SpaceObject obj)
    {
        Objects.Add(obj);
    }
}