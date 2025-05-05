using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Brigade
{
    public int BrigadeId { get; set; }

    public int? BrigadierId { get; set; }

    public string BrigadeName { get; set; } = null!;

    public string BrigadeStatus { get; set; } = null!;

    public virtual Employee? Brigadier { get; set; }

    public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();

    public virtual ICollection<Employee> Workers { get; set; } = new List<Employee>();
}
