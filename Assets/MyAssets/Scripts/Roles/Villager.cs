using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Villager : Role
{
    protected override List<LayerName> LayersAbleToSee => new();
}
