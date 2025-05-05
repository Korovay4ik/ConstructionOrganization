using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class EmployeeAttribute
{
    public int EmployeeAttributeId { get; set; }

    public string AttributeName { get; set; } = null!;

    public virtual ICollection<SpecificEmployeeAttribute> SpecificEmployeeAttributes { get; set; } = new List<SpecificEmployeeAttribute>();
}
