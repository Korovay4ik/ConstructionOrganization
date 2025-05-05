using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewObjectScheduleAndEstimate
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string RecordType { get; set; } = null!;

    public int ScheduleOrMaterialId { get; set; }

    public string? Description { get; set; }

    public string? MaterialName { get; set; }

    public string? UnitOfMeasure { get; set; }

    public DateOnly? PlannedStartDate { get; set; }

    public DateOnly? PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public decimal? PlannedQuantity { get; set; }

    public decimal? ActualQuantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalCost { get; set; }

    public string? AssignedTeam { get; set; }

    public string Status { get; set; } = null!;
}
