using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    protected override List<SigilName> SigilsAbleToSee => Enum.GetValues(typeof(SigilName)).Cast<SigilName>().ToList();
}
