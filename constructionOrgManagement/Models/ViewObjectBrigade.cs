using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewObjectBrigade
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public int BrigadeId { get; set; }

    public string BrigadeName { get; set; } = null!;

    public string BrigadeStatus { get; set; } = null!;

    public string? BrigadierName { get; set; }

    public string CurrentWorkType { get; set; } = null!;

    public DateOnly PlannedStartDate { get; set; }

    public DateOnly PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }
}
