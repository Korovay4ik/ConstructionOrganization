using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class DepartmentEquipment
{
    public int DepartmentEquipmentId { get; set; }

    public int OrgEquipmentId { get; set; }

    public int DepartmentId { get; set; }

    public int DepartEquipmentQuantity { get; set; }

    public virtual ConstructionDepartment Department { get; set; } = null!;

    public virtual ICollection<ObjectEquipment> ObjectEquipments { get; set; } = new List<ObjectEquipment>();

    public virtual OrganizationEquipment OrgEquipment { get; set; } = null!;
}
