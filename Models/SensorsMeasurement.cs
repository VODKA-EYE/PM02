using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class SensorsMeasurement
{
    public int SensorId { get; set; }

    public int TypeId { get; set; }

    public string MeasurmentFormula { get; set; } = null!;

    public virtual Sensor Sensor { get; set; } = null!;

    public virtual MeasurementsType Type { get; set; } = null!;
}
