using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewObjectConstructionReport
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ObjectType { get; set; } = null!;

    public string ConstructionDepartment { get; set; } = null!;

    public string ConstructionSite { get; set; } = null!;

    public string RecordType { get; set; } = null!;

    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? BrigadeName { get; set; }

    public decimal? PlannedQuantity { get; set; }

    public decimal? ActualQuantity { get; set; }

    public string? UnitOfMeasure { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? ActualCost { get; set; }

    public DateOnly? PlannedStartDate { get; set; }

    public DateOnly? PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }
}
