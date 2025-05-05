using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class OrganizationEquipment
{
    public int OrganizationEquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public int OrgEquipmentQuantity { get; set; }

    public virtual ICollection<DepartmentEquipment> DepartmentEquipments { get; set; } = new List<DepartmentEquipment>();
}
