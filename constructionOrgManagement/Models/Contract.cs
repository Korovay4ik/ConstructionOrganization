using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string ContractName { get; set; } = null!;

    public int ContractCustomerId { get; set; }

    public decimal TotalAmount { get; set; }

    public string ContractStatus { get; set; } = null!;

    public virtual Customer ContractCustomer { get; set; } = null!;

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();
}
