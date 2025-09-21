using System.Runtime.Serialization;

namespace MauiApp1.Model;

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