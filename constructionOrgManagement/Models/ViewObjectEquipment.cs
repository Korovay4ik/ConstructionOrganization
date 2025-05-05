using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewObjectEquipment
{
    public int ObjectId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string ObjectCategory { get; set; } = null!;

    public string SiteName { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public int DepartmentEquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public int AssignedQuantity { get; set; }

    public DateOnly? AssignmentDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string AssignmentStatus { get; set; } = null!;
}
