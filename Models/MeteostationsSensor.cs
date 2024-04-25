using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class MeteostationsSensor
{
    public Guid SensorInventoryNumber { get; set; }

    public int StationId { get; set; }

    public int SensorId { get; set; }

    public DateTime AddedTs { get; set; }

    public DateTime? RemovedTs { get; set; }

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    public virtual Sensor Sensor { get; set; } = null!;

    public virtual Meteostation Station { get; set; } = null!;
}
