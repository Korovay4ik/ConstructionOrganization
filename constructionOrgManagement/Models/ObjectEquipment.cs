using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ObjectEquipment
{
    public int EquipmentId { get; set; }

    public int EquipmentForObjectId { get; set; }

    public int EquipObjectQuantity { get; set; }

    public DateOnly? AssignmentDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public virtual DepartmentEquipment Equipment { get; set; } = null!;

    public virtual Object EquipmentForObject { get; set; } = null!;
}
