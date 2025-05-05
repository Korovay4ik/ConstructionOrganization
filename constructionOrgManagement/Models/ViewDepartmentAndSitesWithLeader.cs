using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ViewDepartmentAndSitesWithLeader
{
    public string EntityType { get; set; } = null!;

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public string? LeaderName { get; set; }

    public string LeaderPosition { get; set; } = null!;

    public int? ParentDepartmentId { get; set; }

    public string? ParentDepartmentName { get; set; }
}
