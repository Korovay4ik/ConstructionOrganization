using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class EmployeeCategory
{
    public int EmployeeCategoryId { get; set; }

    public string EmplCategoryName { get; set; } = null!;

    public string CategoryType { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
