using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Object
{
    public int ObjectId { get; set; }

    public int ObjectSiteId { get; set; }

    public int ObjectContractId { get; set; }

    public int CategoryId { get; set; }

    public int? ForemanId { get; set; }

    public string ObjectName { get; set; } = null!;

    public string? ObjectLocation { get; set; }

    public string ObjectStatus { get; set; } = null!;

    public virtual ObjectCategory Category { get; set; } = null!;

    public virtual ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();

    public virtual Employee? Foreman { get; set; }

    public virtual Contract ObjectContract { get; set; } = null!;

    public virtual ICollection<ObjectEquipment> ObjectEquipments { get; set; } = new List<ObjectEquipment>();

    public virtual Site ObjectSite { get; set; } = null!;

    public virtual ICollection<SpecificObjectCharacteristic> SpecificObjectCharacteristics { get; set; } = new List<SpecificObjectCharacteristic>();

    public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();

    public virtual ICollection<Employee> MasterEmployees { get; set; } = new List<Employee>();
}
