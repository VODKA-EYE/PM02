using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class Meteostation
{
    public int StationId { get; set; }

    public string StationName { get; set; } = null!;

    public decimal StationLongitude { get; set; }

    public decimal StationLatitude { get; set; }

    public virtual ICollection<MeteostationsSensor> MeteostationsSensors { get; set; } = new List<MeteostationsSensor>();
}
