using UnityEngine;
using System.Collections.Generic;

public class Medium : Role
{
    [Header("Medium Params")]
    protected override List<SigilName> SigilsAbleToSee => new() { };
}