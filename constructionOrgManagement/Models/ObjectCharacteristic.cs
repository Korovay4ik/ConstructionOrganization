using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class ObjectCharacteristic
{
    public int ObjectCharacteristicId { get; set; }

    public string ObjCharacteristicName { get; set; } = null!;

    public virtual ICollection<SpecificObjectCharacteristic> SpecificObjectCharacteristics { get; set; } = new List<SpecificObjectCharacteristic>();
}
