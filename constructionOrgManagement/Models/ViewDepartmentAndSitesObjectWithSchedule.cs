using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewDepartmentAndSitesObjectWithSchedule
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ObjectCategory { get; set; } = null!;

    public string ObjectStatus { get; set; } = null!;

    public string? ObjectLocation { get; set; }

    public int? ScheduleId { get; set; }

    public string? WorkTypeName { get; set; }

    public DateOnly? PlannedStartDate { get; set; }

    public DateOnly? PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public string WorkStatus { get; set; } = null!;

    public string? AssignedBrigade { get; set; }
}
