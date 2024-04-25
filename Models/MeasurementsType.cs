using System;
using System.Collections.Generic;

namespace PM02.Models;

public partial class MeasurementsType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string TypeUnits { get; set; } = null!;

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}
