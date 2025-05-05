using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int EmplCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Education { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateOnly? FireDate { get; set; }

    public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();

    public virtual ICollection<ConstructionDepartment> ConstructionDepartments { get; set; } = new List<ConstructionDepartment>();

    public virtual EmployeeCategory EmplCategory { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();

    public virtual ICollection<Site> Sites { get; set; } = new List<Site>();

    public virtual ICollection<SpecificEmployeeAttribute> SpecificEmployeeAttributes { get; set; } = new List<SpecificEmployeeAttribute>();

    public virtual ICollection<Brigade> BcBrigades { get; set; } = new List<Brigade>();

    public virtual ICollection<Object> MasterObjects { get; set; } = new List<Object>();
}
