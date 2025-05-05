using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class WorkType
{
    public int WorkTypeId { get; set; }

    public string WorkTypeName { get; set; } = null!;

    public string? WorkTypeDescription { get; set; }

    public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();

    public virtual ICollection<WorkTypeForCategory> WorkTypeForCategories { get; set; } = new List<WorkTypeForCategory>();
}
