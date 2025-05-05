using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Estimate
{
    public int MaterialId { get; set; }

    public int EstimateObjectId { get; set; }

    public decimal PlannedMaterialQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? ActualMaterialQuantity { get; set; }

    public virtual Object EstimateObject { get; set; } = null!;

    public virtual BuildingMaterial Material { get; set; } = null!;
}
