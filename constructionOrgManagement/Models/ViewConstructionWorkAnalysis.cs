using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewConstructionWorkAnalysis
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ObjectCategory { get; set; } = null!;

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public int WorkTypeId { get; set; }

    public string WorkTypeName { get; set; } = null!;

    public int ScheduleId { get; set; }

    public int? BrigadeId { get; set; }

    public string? BrigadeName { get; set; }

    public DateOnly PlannedStartDate { get; set; }

    public DateOnly PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public int? DelayDays { get; set; }

    public int? CurrentDelayDays { get; set; }

    public string WorkStatus { get; set; } = null!;

    public string DeadlineStatus { get; set; } = null!;
}
