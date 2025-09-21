namespace MauiApp1.Model;

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
        var randomArrivalTime = DateTime.Now.AddDays(random.Next(1, 30))
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
            Temperature = GenerateRandomTemperature(),
            MagneticField = random.NextDouble() * 0.5, // до 0.5 Тесла
            EmissionFrequency = random.NextDouble() * 1e15 + 1e10, // 10 GHz - 1 PHz
            Wavelength = random.NextDouble() * 5000 + 10, // 10-5010 метров
            Radiation = GetRandomRadiationLevel(),
            Size = GenerateRandomSize()
        }; 

        // Определяем тип объекта и уровень угрозы
        ClassifyObject(obj);

        return obj;
    }

    private static double GenerateRandomTemperature()
    {
        // Температура в Кельвинах с реалистичным распределением
        // 1. Околоземное пространство: 250-320 K (-23°C to +47°C) - 60%
        // 2. Холодное космическое пространство: 3-100 K (-270°C to -173°C) - 30%
        // 3. Околозвездное пространство: 320-1000 K (+47°C to +727°C) - 8%
        // 4. Экстремально горячее: 1000-5000 K (+727°C to +4727°C) - 2%

        // Средняя температура около Земли: 283.32 K (10.17°C)
        const double earthOrbitTemp = 283.32;

        var category = random.Next(100);

        return category switch
        {
            // Околоземное пространство (нормальное распределение вокруг 283.32K)
            < 60 => GenerateNormalDistribution(earthOrbitTemp, 20, 250, 320),

            // Холодное космическое пространство
            < 90 => random.NextDouble() * 97 + 3,

            // Околозвездное пространство
            < 98 => random.NextDouble() * 680 + 320,

            // Экстремально горячее (возле звезд, в активных зонах)
            _ => random.NextDouble() * 4000 + 1000
        };
    }

    // Вспомогательный метод для нормального распределения
    private static double GenerateNormalDistribution(double mean, double stdDev, double min, double max)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        double randNormal = mean + stdDev * randStdNormal;

        return Math.Max(min, Math.Min(max, randNormal));
    }

    private static double GenerateRandomSize()
    {
        // Размер в метрах с ограничениями для естественных объектов
        // 1. Очень маленькие: 1-5 м (45%) - космический мусор, мелкие метеориты
        // 2. Маленькие: 5-50 м (30%) - средние метеориты, спутники
        // 3. Средние: 50-200 м (20%) - крупные метеориты, небольшие астероиды
        // 4. Крупные: 200-500 м (5%) - астероиды (максимум для средней угрозы)

        var category = random.Next(100);

        return category switch
        {
            < 45 => random.NextDouble() * 4 + 1,           // 45%: 1-5 м
            < 75 => random.NextDouble() * 45 + 5,          // 30%: 5-50 м
            < 95 => random.NextDouble() * 150 + 50,        // 20%: 50-200 м
            _ => random.NextDouble() * 300 + 200           // 5%: 200-500 м
        };
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
        // Скорость в км/ч с ограничениями для естественных объектов
        // 1. Медленные: 100-2000 км/ч (30%)
        // 2. Средние: 2000-8000 км/ч (50%)
        // 3. Быстрые: 8000-15000 км/ч (18%)
        // 4. Очень быстрые: 15000-20000 км/ч (2%) - максимум для средней угрозы

        var category = random.Next(100);

        return category switch
        {
            < 30 => random.NextDouble() * 1900 + 100,      // 30%: 100-2000 км/ч
            < 80 => random.NextDouble() * 6000 + 2000,     // 50%: 2000-8000 км/ч
            < 98 => random.NextDouble() * 7000 + 8000,     // 18%: 8000-15000 км/ч
            _ => random.NextDouble() * 5000 + 15000        // 2%: 15000-20000 км/ч
        };
    }

    private static RadiationLevel GetRandomRadiationLevel()
    {
        // Радиация с ограничениями для естественных объектов
        // Естественные объекты не могут иметь экстремальную радиацию
        var weights = new Dictionary<RadiationLevel, int>
    {
        { RadiationLevel.None, 70 },     // 70% chance
        { RadiationLevel.Low, 20 },      // 20% chance
        { RadiationLevel.Medium, 10 },   // 10% chance
    };

        return GetWeightedRandom(weights);
    }

    // Для кораблей отдельная генерация параметров
    private static SpaceObject GenerateSpaceshipParameters(SpaceObject obj)
    {
        // Корабли могут иметь особые параметры
        obj.Size = random.NextDouble() * 40 + 10; // 10-50 м
        obj.Speed = random.Next(100) < 30 ? 0 : random.NextDouble() * 18000 + 2000; // 0 или 2000-20000
        obj.Radiation = GetSpaceshipRadiation();
        obj.MagneticField = random.NextDouble() * 0.4f + 0.1f; // 0.1-0.5 Тесла
        obj.EmissionFrequency = random.NextDouble() * 1e13 + 1e11; // 100 GHz - 10 THz
        obj.Temperature = random.NextDouble() * 100 + 280; // 280-380 K

        return obj;
    }

    private static RadiationLevel GetSpaceshipRadiation()
    {
        // Корабли могут иметь любую радиацию
        var weights = new Dictionary<RadiationLevel, int>
    {
        { RadiationLevel.None, 50 },
        { RadiationLevel.Low, 30 },
        { RadiationLevel.Medium, 15 },
        { RadiationLevel.High, 4 },
        { RadiationLevel.Extreme, 1 }
    };

        return GetWeightedRandom(weights);
    }

    private static ShapeType GetRandomShape()
    {
        var weights = new Dictionary<ShapeType, int>
    {
        { ShapeType.Irregular, 50 },   // 50% - большинство объектов неправильной формы
        { ShapeType.Oval, 15 },        // 15% 
        { ShapeType.OblongOval, 10 },  // 10%
        { ShapeType.Circle, 8 },       // 8%
        { ShapeType.Sphere, 8 },       // 8%
        { ShapeType.Triangle, 4 },     // 4% 
        { ShapeType.Rectangle, 5 }     // 5%
    };

        return GetWeightedRandom(weights);
    }

    private static void ClassifyObject(SpaceObject obj)
    {
        // Сначала проверяем корабль (самые строгие критерии)
        if (IsSpaceship(obj))
        {
            obj.Type = ObjectType.Spaceship;
            GenerateSpaceshipParameters(obj); // Перегенерируем параметры для корабля
        }
        else if (IsSpaceDebris(obj))
        {
            obj.Type = ObjectType.SpaceDebris;
            ApplyNaturalLimits(obj); // Применяем ограничения
        }
        else if (IsMeteorite(obj))
        {
            obj.Type = ObjectType.Meteorite;
            ApplyNaturalLimits(obj);
        }
        else if (IsAsteroid(obj))
        {
            obj.Type = ObjectType.Asteroid;
            ApplyNaturalLimits(obj);
        }
        else if (IsComet(obj))
        {
            obj.Type = ObjectType.Comet;
            ApplyNaturalLimits(obj);
        }
        else if (IsSatellite(obj))
        {
            obj.Type = ObjectType.Satellite;
            ApplyNaturalLimits(obj);
        }
        else
        {
            obj.Type = ObjectType.Unknown;
            ApplyNaturalLimits(obj);
        }

        obj.Threat = CalculateThreatLevel(obj);
        obj.IsThreat = obj.Threat >= ThreatLevel.LowThreat;
    }
    private static void ApplyNaturalLimits(SpaceObject obj)
    {
        // Принудительно ограничиваем параметры естественных объектов
        if (obj.Size > 500) obj.Size = 500; // Максимум 500 м
        if (obj.Speed > 20000) obj.Speed = 20000; // Максимум 20000 км/ч
        if (obj.Radiation > RadiationLevel.Medium)
            obj.Radiation = RadiationLevel.Medium; // Максимум средняя радиация
        if (obj.MagneticField > 0.2f) obj.MagneticField = 0.2f; // Максимум 0.2 Тесла
    }

    private static ThreatLevel CalculateThreatLevel(SpaceObject obj)
    {
        // Для естественных объектов максимальная угроза - MediumThreat
        if (obj.Type != ObjectType.Spaceship)
        {
            return CalculateNaturalThreatLevel(obj);
        }

        // Для кораблей полная система оценки
        return CalculateSpaceshipThreatLevel(obj);
    }


    private static ThreatLevel CalculateNaturalThreatLevel(SpaceObject obj)
    {
        int threatScore = 0;

        // Ограниченная система оценки для естественных объектов
        if (obj.Size > 300) threatScore += 2;
        else if (obj.Size > 100) threatScore += 1;

        if (obj.Speed > 15000) threatScore += 2;
        else if (obj.Speed > 10000) threatScore += 1;

        if (obj.Radiation >= RadiationLevel.Medium) threatScore += 1;

        // Естественные объекты не могут получить больше MediumThreat
        return threatScore switch
        {
            >= 4 => ThreatLevel.MediumThreat,     // Максимум для естественных
            >= 2 => ThreatLevel.LowThreat,
            _ => ThreatLevel.Safe
        };
    }
    private static ThreatLevel CalculateSpaceshipThreatLevel(SpaceObject obj)
    {
        int threatScore = 2;

        // Полная система оценки для кораблей
        if (obj.Size > 30) threatScore += 1;
        if (obj.Speed == 0) threatScore += 2; // Неподвижный корабль подозрителен
        if (obj.Radiation >= RadiationLevel.High) threatScore += 2;
        if (obj.Radiation >= RadiationLevel.Medium) threatScore += 1;
        if (obj.MagneticField > 0.3f) threatScore += 1;
        if (obj.EmissionFrequency > 5e12) threatScore += 1;

        return threatScore switch
        {
            >= 6 => ThreatLevel.CriticalThreat,
            >= 4 => ThreatLevel.HighThreat,
            >= 2 => ThreatLevel.MediumThreat,
            >= 1 => ThreatLevel.LowThreat,
            _ => ThreatLevel.Safe
        };
    }

    private static bool IsSpaceship(SpaceObject obj)
    {
        // Упрощаем критерии и увеличиваем шанс обнаружения кораблей

        bool isArtificialShape = obj.Shape == ShapeType.Triangle ||
                               obj.Shape == ShapeType.Rectangle ||
                               (obj.Shape == ShapeType.Sphere && obj.Size < 50);

        bool hasReasonableSize = obj.Size > 5 && obj.Size < 80;

        bool hasArtificialFeatures =
            obj.MagneticField > 0.05f || // Слабое магнитное поле тоже может быть искусственным
            (obj.EmissionFrequency > 1e11 && obj.EmissionFrequency < 1e15) || // Шире диапазон
            (obj.Temperature > 250 && obj.Temperature < 400) || // Комфортная температура
            obj.Radiation <= RadiationLevel.Low; // Низкая радиация

        bool hasControlledMovement =
            obj.Speed == 0 || // Стационарное положение
            (obj.Speed > 1000 && obj.Speed < 25000); // Контролируемая скорость

        // Более мягкие критерии - достаточно нескольких признаков
        int score = 0;
        if (isArtificialShape) score += 2;
        if (hasReasonableSize) score += 1;
        if (hasArtificialFeatures) score += 2;
        if (hasControlledMovement) score += 1;

        // Увеличиваем шанс до 8% (было 3%)
        return score >= 3 && random.Next(100) < 8;
    }

    private static bool HasArtificialFeatures(SpaceObject obj)
    {
        // Вспомогательный метод для проверки искусственных признаков
        return (obj.Shape == ShapeType.Triangle || obj.Shape == ShapeType.Rectangle) ||
               (obj.MagneticField > 0.05f) ||
               (obj.EmissionFrequency > 1e11 && obj.EmissionFrequency < 1e14) ||
               (obj.Temperature > 250 && obj.Temperature < 350) ||
               obj.Radiation == RadiationLevel.None;
    }

    private static bool IsMeteorite(SpaceObject obj)
    {
        // Более широкие критерии для метеоритов
        return (obj.Shape == ShapeType.Irregular || obj.Shape == ShapeType.OblongOval) &&
               obj.Speed > 5000 &&
               obj.Density > 1000 &&
               obj.Size < 200;
    }


    private static bool IsAsteroid(SpaceObject obj)
    {
        // Более широкие критерии для астероидов
        return obj.Size >= 50 &&
               (obj.Shape == ShapeType.Irregular || obj.Shape == ShapeType.Oval ||
                obj.Shape == ShapeType.Sphere) &&
               obj.Distance > 50000;
    }

    private static bool IsComet(SpaceObject obj)
    {
        // Более широкие критерии для комет
        return obj.Density < 1200 &&
               (obj.Shape == ShapeType.OblongOval || obj.Shape == ShapeType.Irregular) &&
               obj.Speed > 3000;
    }

    private static bool IsSatellite(SpaceObject obj)
    {
        // Более широкие критерии для спутников
        return obj.Size < 25 &&
               (obj.Shape == ShapeType.Circle || obj.Shape == ShapeType.Sphere ||
                obj.Shape == ShapeType.Rectangle) &&
               obj.Speed > 5000 && obj.Speed < 20000 &&
               obj.Distance < 100000;
    }

    private static bool IsSpaceDebris(SpaceObject obj)
    {
        // Космический мусор - самый частый тип
        return obj.Size < 15 &&
               obj.Shape == ShapeType.Irregular &&
               obj.Speed > 1000 && obj.Speed < 35000;
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