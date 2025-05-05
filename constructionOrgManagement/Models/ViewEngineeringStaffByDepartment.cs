using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewEngineeringStaffByDepartment
{
    public int EmployeeId { get; set; }

    public string? FullName { get; set; }

    public string Position { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string? Site { get; set; }

    public DateOnly? FireDate { get; set; }

    public string? ContactNumber { get; set; }
}
