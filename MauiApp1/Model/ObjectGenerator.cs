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

        return category switch
        {
            < 10 => random.NextDouble() * 900 + 100,
            < 50 => random.NextDouble() * 9000 + 1000,
            < 80 => random.NextDouble() * 40000 + 10000,
            _ => random.NextDouble() * 150000 + 50000
        };
    }

    private static ShapeType GetRandomShape()
    {
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