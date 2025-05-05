using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ConstructionDepartment
{
    public int ConstructionDepartmentId { get; set; }

    public int? DepartmentSupervisorId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? DepartmentLocation { get; set; }

    public virtual ICollection<DepartmentEquipment> DepartmentEquipments { get; set; } = new List<DepartmentEquipment>();

    public virtual Employee? DepartmentSupervisor { get; set; }

    public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
}
