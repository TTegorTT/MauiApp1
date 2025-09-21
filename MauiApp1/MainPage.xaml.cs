using System.Collections.ObjectModel;
using System.Globalization;
using System.Timers;
using MauiApp1.Converters;
using MauiApp1.P2P;

namespace MauiApp1
{
    public static class ObjectGenerator
    {
        private static Random random = new Random();
        private static List<string> coordinatePrefixes = new List<string>
        {
            "N", "S", "E", "W", "NE", "NW", "SE", "SW"
        };

        public static SpaceObject GenerateRandomObject()
        {
            // Генерируем случайное время прибытия (от -7 до +30 дней)
            var randomArrivalTime = DateTime.Now.AddDays(random.Next(-7, 30))
                                                .AddHours(random.Next(0, 24))
                                                .AddMinutes(random.Next(0, 60))
                                                .AddSeconds(random.Next(0, 60));

            var obj = new SpaceObject
            {
                ArrivalTime = randomArrivalTime,
                Coordinates = GenerateRandomCoordinates(),
                Distance = GenerateRandomDistance(), // Новый метод для расстояния
                Speed = GenerateRandomSpeed(),       // Новый метод для скорости
                Shape = GetRandomShape(),
                Density = random.NextDouble() * 8000 + 500, // 500-8500 кг/м³
                Temperature = random.NextDouble() * 1500 - 200, // -200 до +1300 °C
                MagneticField = random.NextDouble() * 0.5, // до 0.5 Тесла
                EmissionFrequency = random.NextDouble() * 1e15 + 1e10, // 10 GHz - 1 PHz
                Wavelength = random.NextDouble() * 5000 + 10, // 10-5010 метров
                Radiation = GetRandomRadiationLevel(),
                Size = random.NextDouble() * 2000 + 1 // 1-2001 метров
            };

            // Определяем тип объекта и уровень угрозы
            ClassifyObject(obj);

            return obj;
        }

        private static double GenerateRandomDistance()
        {
            // Расстояние в километрах (от тысяч до миллионов км)
            // 1. Ближний космос: 1000-100000 км (20%)
            // 2. Средний космос: 100000-1000000 км (50%)
            // 3. Дальний космос: 1-10 млн км (20%)
            // 4. Межпланетный: 10-100 млн км (10%)

            var category = random.Next(100);

            if (category < 20) return random.NextDouble() * 99000 + 1000; // 1000-100000 км
            if (category < 70) return random.NextDouble() * 900000 + 100000; // 100000-1000000 км
            if (category < 90) return random.NextDouble() * 9000000 + 1000000; // 1-10 млн км
            return random.NextDouble() * 90000000 + 10000000; // 10-100 млн км
        }

        private static double GenerateRandomSpeed()
        {
            // Скорость в км/ч (от сотен до десятков тысяч км/с)
            // 1. Медленные: 100-1000 км/ч (10%)
            // 2. Средние: 1000-10000 км/ч (40%)
            // 3. Быстрые: 10000-50000 км/ч (30%)
            // 4. Очень быстрые: 50000-200000 км/ч (20%)

            var category = random.Next(100);

            if (category < 10) return random.NextDouble() * 900 + 100; // 100-1000 км/ч
            if (category < 50) return random.NextDouble() * 9000 + 1000; // 1000-10000 км/ч
            if (category < 80) return random.NextDouble() * 40000 + 10000; // 10000-50000 км/ч
            return random.NextDouble() * 150000 + 50000; // 50000-200000 км/ч
        }

        private static ShapeType GetRandomShape()
        {
            // Делаем более естественное распределение форм
            var shapes = new List<ShapeType>
            {
                ShapeType.Irregular,    // 30% chance
                ShapeType.Irregular,
                ShapeType.Irregular,
                ShapeType.Oval,         // 20% chance
                ShapeType.Oval,
                ShapeType.OblongOval,   // 15% chance
                ShapeType.Circle,       // 15% chance
                ShapeType.Sphere,       // 10% chance
                ShapeType.Triangle,     // 5% chance
                ShapeType.Rectangle     // 5% chance
            };

            return shapes[random.Next(shapes.Count)];
        }

        private static RadiationLevel GetRandomRadiationLevel()
        {
            // Более естественное распределение радиации
            var weights = new Dictionary<RadiationLevel, int>
            {
                { RadiationLevel.None, 60 },     // 60% chance
                { RadiationLevel.Low, 25 },      // 25% chance
                { RadiationLevel.Medium, 10 },   // 10% chance
                { RadiationLevel.High, 4 },      // 4% chance
                { RadiationLevel.Extreme, 1 }    // 1% chance
            };

            return GetWeightedRandom(weights);
        }

        private static void ClassifyObject(SpaceObject obj)
        {
            // Сначала определяем естественные объекты
            if (IsMeteorite(obj))
            {
                obj.Type = ObjectType.Meteorite;
            }
            else if (IsAsteroid(obj))
            {
                obj.Type = ObjectType.Asteroid;
            }
            else if (IsComet(obj))
            {
                obj.Type = ObjectType.Comet;
            }
            else if (IsSatellite(obj))
            {
                obj.Type = ObjectType.Satellite;
            }
            else if (IsSpaceDebris(obj))
            {
                obj.Type = ObjectType.SpaceDebris;
            }
            // Искусственные объекты определяем в последнюю очередь
            else if (IsSpaceship(obj))
            {
                obj.Type = ObjectType.Spaceship;
            }
            else
            {
                obj.Type = ObjectType.Unknown;
            }

            // Определяем уровень угрозы (более сбалансированно)
            obj.Threat = CalculateThreatLevel(obj);
            obj.IsThreat = obj.Threat >= ThreatLevel.LowThreat;
        }

        private static bool IsSpaceship(SpaceObject obj)
        {
            // Более строгие критерии для кораблей (редкие)
            bool isGeometricShape = obj.Shape == ShapeType.Triangle ||
                                  obj.Shape == ShapeType.Rectangle ||
                                  (obj.Shape == ShapeType.Sphere && obj.Size > 20 && obj.Size < 100);

            bool hasArtificialFeatures = obj.Speed == 0 || // Висит в пространстве
                                      obj.MagneticField > 0.2f || // Сильное магнитное поле
                                      obj.EmissionFrequency > 5e12 || // Высокочастотное излучение
                                      (obj.Temperature > 100 && obj.Temperature < 500); // Контролируемая температура

            // Только если ОБА условия выполнены + случайный фактор (5% шанс)
            return isGeometricShape && hasArtificialFeatures && random.Next(100) < 5;
        }

        private static bool IsMeteorite(SpaceObject obj)
        {
            // Метеориты: высокая скорость, неправильная форма
            return (obj.Shape == ShapeType.Irregular || obj.Shape == ShapeType.OblongOval) &&
                   obj.Speed > 10000 &&
                   obj.Density > 2000;
        }

        private static bool IsAsteroid(SpaceObject obj)
        {
            // Астероиды: большие размеры, неправильная форма
            return obj.Size > 200 &&
                   (obj.Shape == ShapeType.Irregular || obj.Shape == ShapeType.Oval);
        }

        private static bool IsComet(SpaceObject obj)
        {
            // Кометы: низкая плотность, вытянутая форма, высокая скорость
            return obj.Density < 800 &&
                   obj.Shape == ShapeType.OblongOval &&
                   obj.Speed > 15000;
        }

        private static bool IsSatellite(SpaceObject obj)
        {
            // Спутники: небольшие размеры, средняя скорость, круговая форма
            return obj.Size < 15 &&
                   obj.Speed > 5000 && obj.Speed < 25000 &&
                   (obj.Shape == ShapeType.Circle || obj.Shape == ShapeType.Sphere);
        }

        private static bool IsSpaceDebris(SpaceObject obj)
        {
            // Космический мусор: маленький размер, неправильная форма
            return obj.Size < 5 &&
                   obj.Shape == ShapeType.Irregular &&
                   obj.Speed > 1000 && obj.Speed < 30000;
        }

        private static ThreatLevel CalculateThreatLevel(SpaceObject obj)
        {
            int threatScore = 0;

            // Более сбалансированные критерии угрозы
            if (obj.Type == ObjectType.Spaceship) threatScore += 2;
            if (obj.Speed == 0) threatScore += 1; // Висит в пространстве
            if (obj.Shape == ShapeType.Triangle) threatScore += 2; // Четкий треугольник

            // Большие шары реже являются угрозой
            if (obj.Shape == ShapeType.Sphere && obj.Size >= 500) threatScore += 1;
            if (obj.Shape == ShapeType.Sphere && obj.Size >= 1000) threatScore += 1;

            // Учитываем расстояние (близкие объекты опаснее)
            if (obj.Distance < 10000) threatScore += 3; // Очень близко (<10k км)
            else if (obj.Distance < 100000) threatScore += 2; // Близко (<100k км)
            else if (obj.Distance < 1000000) threatScore += 1; // Средняя дистанция

            if (obj.Radiation >= RadiationLevel.High) threatScore += 1;
            if (obj.Radiation >= RadiationLevel.Extreme) threatScore += 2;

            if (obj.Size > 1000) threatScore += 2; // Очень крупный объект
            if (obj.Size > 500) threatScore += 1; // Крупный объект

            // Направление движения (условно)
            bool isApproaching = random.Next(100) < 30; // 30% шанс что приближается
            if (isApproaching) threatScore += 1;

            return threatScore switch
            {
                >= 6 => ThreatLevel.CriticalThreat,    // 5%
                >= 4 => ThreatLevel.HighThreat,        // 10%
                >= 3 => ThreatLevel.MediumThreat,      // 15%
                >= 2 => ThreatLevel.LowThreat,         // 20%
                _ => ThreatLevel.Safe                  // 50%
            };
        }

        private static T GetWeightedRandom<T>(Dictionary<T, int> weights)
        {
            int totalWeight = 0;
            foreach (var weight in weights.Values)
            {
                totalWeight += weight;
            }

            int randomNumber = random.Next(totalWeight);
            int currentWeight = 0;

            foreach (var entry in weights)
            {
                currentWeight += entry.Value;
                if (randomNumber < currentWeight)
                {
                    return entry.Key;
                }
            }

            return default(T);
        }

        private static string GenerateRandomCoordinates()
        {
            var prefix = coordinatePrefixes[random.Next(coordinatePrefixes.Count)];
            var lat = random.Next(0, 90);
            var lon = random.Next(0, 180);
            var latMinutes = random.Next(0, 60);
            var lonMinutes = random.Next(0, 60);

            return $"{prefix} {lat}°{latMinutes:00}' {lon}°{lonMinutes:00}'";
        }

        public static string FormatDistance(double distanceKm)
        {
            if (distanceKm < 1000) return $"{distanceKm:F0} км";
            if (distanceKm < 1000000) return $"{distanceKm / 1000:F0} тыс. км";
            return $"{distanceKm / 1000000:F1} млн км";
        }

        public static string FormatSpeed(double speedKmh)
        {
            if (speedKmh < 1000) return $"{speedKmh:F0} км/ч";
            if (speedKmh < 10000) return $"{speedKmh / 1000:F1} тыс. км/ч";
            return $"{speedKmh / 1000:F0} тыс. км/ч";
        }

        public static string GetShapeName(ShapeType shape)
        {
            return shape switch
            {
                ShapeType.Oval => "Овал",
                ShapeType.OblongOval => "Продолговатый овал",
                ShapeType.Triangle => "Треугольник",
                ShapeType.Rectangle => "Прямоугольник",
                ShapeType.Circle => "Круг",
                ShapeType.Sphere => "Шар",
                ShapeType.Irregular => "Неправильная форма",
                _ => "Неизвестно"
            };
        }

        public static string GetRadiationName(RadiationLevel radiation)
        {
            return radiation switch
            {
                RadiationLevel.None => "Отсутствует",
                RadiationLevel.Low => "Низкий",
                RadiationLevel.Medium => "Средний",
                RadiationLevel.High => "Высокий",
                RadiationLevel.Extreme => "Экстремальный",
                _ => "Неизвестно"
            };
        }

        public static string GetObjectTypeName(ObjectType type)
        {
            return type switch
            {
                ObjectType.Meteorite => "Метеорит",
                ObjectType.Spaceship => "Космический корабль",
                ObjectType.Satellite => "Спутник",
                ObjectType.SpaceDebris => "Космический мусор",
                ObjectType.Asteroid => "Астероид",
                ObjectType.Comet => "Комета",
                _ => "Неизвестный объект"
            };
        }

        public static string GetThreatLevelName(ThreatLevel threat)
        {
            return threat switch
            {
                ThreatLevel.Safe => "Безопасен",
                ThreatLevel.LowThreat => "Низкая угроза",
                ThreatLevel.MediumThreat => "Средняя угроза",
                ThreatLevel.HighThreat => "Высокая угроза",
                ThreatLevel.CriticalThreat => "Критическая угроза",
                _ => "Неизвестно"
            };
        }

        public static Color GetThreatColor(ThreatLevel threat)
        {
            return threat switch
            {
                ThreatLevel.Safe => Colors.Green,
                ThreatLevel.LowThreat => Colors.Blue,
                ThreatLevel.MediumThreat => Colors.Orange,
                ThreatLevel.HighThreat => Colors.OrangeRed,
                ThreatLevel.CriticalThreat => Colors.Red,
                _ => Colors.Gray
            };
        }
    }
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

            _p2pService = new P2PService();
            _p2pService.PeerDiscovered += OnPeerDiscovered;
            _p2pService.ThreatReceived += OnThreatReceived;
            _p2pService.LogMessage += OnP2PLogMessage;

            // Автоматически запускаем P2P
            _ = StartP2PService();

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
            if (sender is Frame frame && frame.BindingContext is SpaceObject spaceObject)
            {
                await ShowObjectDetails(spaceObject);
            }
        }

        private async Task ShowObjectDetails(SpaceObject obj)
        {
            string threatEmoji = obj.Threat switch
            {
                ThreatLevel.CriticalThreat => "💀",
                ThreatLevel.HighThreat => "🔥",
                ThreatLevel.MediumThreat => "⚠️",
                ThreatLevel.LowThreat => "🔶",
                _ => "✅"
            };

            string details = $"""
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

    // Модель данных для элемента списка
    public enum ObjectType
    {
        Unknown,
        Meteorite,
        Spaceship,
        Satellite,
        SpaceDebris,
        Asteroid,
        Comet
    }

    public enum ThreatLevel
    {
        Safe,
        LowThreat,
        MediumThreat,
        HighThreat,
        CriticalThreat
    }

    public enum ShapeType
    {
        Oval,
        OblongOval,
        Triangle,
        Rectangle,
        Circle,
        Irregular,
        Sphere
    }

    public enum RadiationLevel
    {
        None,
        Low,
        Medium,
        High,
        Extreme
    }

    public class SpaceObject
    {
        public DateTime ArrivalTime { get; set; }
        public string Coordinates { get; set; }
        public double Distance { get; set; } // метры
        public double Speed { get; set; } // км/ч
        public ShapeType Shape { get; set; }
        public double Density { get; set; } // кг/м³
        public double Temperature { get; set; } // градусы Цельсия
        public double MagneticField { get; set; } // Тесла
        public double EmissionFrequency { get; set; } // Гц
        public double Wavelength { get; set; } // метры
        public RadiationLevel Radiation { get; set; }
        public double Size { get; set; } // метры (диаметр/размер)
        public ObjectType Type { get; set; }
        public ThreatLevel Threat { get; set; }
        public bool IsThreat { get; set; }

        public string FormattedDistance
        {
            get
            {
                if (Distance < 1000) return $"{Distance:F0} км";
                if (Distance < 1000000) return $"{Distance / 1000:F0} тыс. км";
                if (Distance < 1000000000) return $"{Distance / 1000000:F1} млн км";
                return $"{Distance / 1000000000:F1} млрд км";
            }
        }

        public string FormattedSpeed
        {
            get
            {
                if (Speed < 1000) return $"{Speed:F0} км/ч";
                if (Speed < 10000) return $"{Speed / 1000:F1} тыс. км/ч";
                return $"{Speed / 1000:F0} тыс. км/ч";
            }
        }

        public string ArrivalDate => ArrivalTime.ToString("dd.MM.yyyy");
        public string ArrivalTimeFormatted => ArrivalTime.ToString("HH:mm");
        public string ArrivalFull => $"{ArrivalDate} {ArrivalTimeFormatted}";

        
    }
}
