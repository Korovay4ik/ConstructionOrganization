using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class BuildingMaterial
{
    public int BuildingMaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string UnitOfMeasure { get; set; } = null!;

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();
}
