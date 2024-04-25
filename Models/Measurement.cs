using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class Measurement
{
    public Guid SensorInventoryNumber { get; set; }

    public decimal MeasurementValue { get; set; }

    public DateTime MeasurementTs { get; set; }

    public int MeasurementType { get; set; }

    public int MeasurementId { get; set; }

    public virtual MeasurementsType MeasurementTypeNavigation { get; set; } = null!;

    public virtual MeteostationsSensor SensorInventoryNumberNavigation { get; set; } = null!;
}
