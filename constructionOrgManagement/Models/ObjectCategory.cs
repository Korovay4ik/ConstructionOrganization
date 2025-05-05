using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ObjectCategory
{
    public int ObjectCategoryId { get; set; }

    public string ObjCategoryName { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<WorkTypeForCategory> WorkTypeForCategories { get; set; } = new List<WorkTypeForCategory>();
}
