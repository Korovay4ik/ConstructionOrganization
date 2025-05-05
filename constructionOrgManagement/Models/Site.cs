using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Site
{
    public int SiteId { get; set; }

    public int SiteDepartmentId { get; set; }

    public int? SiteSupervisorId { get; set; }

    public string SiteName { get; set; } = null!;

    public string? SiteLocation { get; set; }

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ConstructionDepartment SiteDepartment { get; set; } = null!;

    public virtual Employee? SiteSupervisor { get; set; }
}
