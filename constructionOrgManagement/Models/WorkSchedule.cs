using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class WorkSchedule
{
    public int ScheduleId { get; set; }

    public int ScheduleObjectId { get; set; }

    public int ScheduleBrigadeId { get; set; }

    public int ScheduleWorkTypeId { get; set; }

    public DateOnly PlannedStartDate { get; set; }

    public DateOnly PlannedEndDate { get; set; }

    public DateOnly? ActualStartDate { get; set; }

    public DateOnly? ActualEndDate { get; set; }

    public virtual Brigade ScheduleBrigade { get; set; } = null!;

    public virtual Object ScheduleObject { get; set; } = null!;

    public virtual WorkType ScheduleWorkType { get; set; } = null!;
}
