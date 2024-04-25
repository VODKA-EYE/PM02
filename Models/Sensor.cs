using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class Sensor
{
    public int SensorId { get; set; }

    public string SensorName { get; set; } = null!;

    public virtual ICollection<MeteostationsSensor> MeteostationsSensors { get; set; } = new List<MeteostationsSensor>();
}
