using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Model;

namespace MauiApp1.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336"); // Green : Red
            }
            return Color.FromArgb("#9E9E9E"); // Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double distanceKm)
            {
                return FormatDistance(distanceKm);
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string FormatDistance(double distanceKm)
        {
            if (distanceKm < 1000) return $"{distanceKm:F0} км";
            if (distanceKm < 1000000) return $"{distanceKm / 1000:F0} тыс. км";
            if (distanceKm < 1000000000) return $"{distanceKm / 1000000:F1} млн км";
            return $"{distanceKm / 1000000000:F1} млрд км";
        }
    }

    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double speedKmh)
            {
                return FormatSpeed(speedKmh);
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string FormatSpeed(double speedKmh)
        {
            if (speedKmh < 1000) return $"{speedKmh:F0} км/ч";
            if (speedKmh < 10000) return $"{speedKmh / 1000:F1} тыс. км/ч";
            return $"{speedKmh / 1000:F0} тыс. км/ч";
        }
    }
    public class ThreatToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ThreatLevel threat)
            {
                return threat switch
                {
                    ThreatLevel.Safe => Color.FromArgb("#2ECC71"), // Green
                    ThreatLevel.LowThreat => Color.FromArgb("#3498DB"), // Blue
                    ThreatLevel.MediumThreat => Color.FromArgb("#F39C12"), // Orange
                    ThreatLevel.HighThreat => Color.FromArgb("#E74C3C"), // Red
                    ThreatLevel.CriticalThreat => Color.FromArgb("#C0392B"), // Dark Red
                    _ => Color.FromArgb("#7F8C8D") // Gray
                };
            }
            return Color.FromArgb("#7F8C8D");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObjectType type)
            {
                return type switch
                {
                    ObjectType.Spaceship => "🚀",
                    ObjectType.Meteorite => "☄️",
                    ObjectType.Satellite => "🛰️",
                    ObjectType.Asteroid => "🌑",
                    ObjectType.Comet => "☄️",
                    ObjectType.SpaceDebris => "🗑️",
                    _ => "❓"
                };
            }
            return "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObjectType type)
            {
                return ObjectGenerator.GetObjectTypeName(type);
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThreatToEmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ThreatLevel threat)
            {
                return threat switch
                {
                    ThreatLevel.CriticalThreat => "💀",
                    ThreatLevel.HighThreat => "🔥",
                    ThreatLevel.MediumThreat => "⚠️",
                    ThreatLevel.LowThreat => "🔶",
                    _ => "✅"
                };
            }
            return "✅";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThreatToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ThreatLevel threat)
            {
                return ObjectGenerator.GetThreatLevelName(threat);
            }
            return "Безопасен";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
