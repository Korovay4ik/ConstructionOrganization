using System;
using System.Collections.Generic;

namespace constructionOrgManagement.Models;

public partial class SpecificObjectCharacteristic
{
    public int SpecificObjectId { get; set; }

    public int CharacteristicId { get; set; }

    public string CharacteristicValue { get; set; } = null!;

    public virtual ObjectCharacteristic Characteristic { get; set; } = null!;

    public virtual Object SpecificObject { get; set; } = null!;
}
