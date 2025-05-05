using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewMaterialReport
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string UnitOfMeasure { get; set; } = null!;

    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ObjectCategory { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public decimal PlannedMaterialQuantity { get; set; }

    public decimal? ActualMaterialQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal PlannedCost { get; set; }

    public decimal? ActualCost { get; set; }

    public decimal? OverbudgetPercent { get; set; }

    public string BudgetStatus { get; set; } = null!;
}
