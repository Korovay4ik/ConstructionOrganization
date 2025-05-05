using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class WorkTypeForCategory
{
    public int SpecificCategoryId { get; set; }

    public int WtfcWorkTypeId { get; set; }

    public bool? IsMandatory { get; set; }

    public virtual ObjectCategory SpecificCategory { get; set; } = null!;

    public virtual WorkType WtfcWorkType { get; set; } = null!;
}
