using UnityEngine;
using System.Collections.Generic;

public class Medium : Role
{
    protected override List<LayerName> LayersAbleToSee => new();
}