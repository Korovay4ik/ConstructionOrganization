using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class SpecificEmployeeAttribute
{
    public int AttributeId { get; set; }

    public int SpecificEmployeeId { get; set; }

    public string AttributeValue { get; set; } = null!;

    public virtual EmployeeAttribute Attribute { get; set; } = null!;

    public virtual Employee SpecificEmployee { get; set; } = null!;
}
