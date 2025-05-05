using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewDepartmentEquipmentList
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int DepartmentEquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public int AssignedQuantity { get; set; }

    public int? AvailableDepartmentQuantity { get; set; }

    public int TotalInOrganization { get; set; }
}
