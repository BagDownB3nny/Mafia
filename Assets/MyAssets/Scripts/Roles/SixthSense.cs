using System.Collections.Generic;
using System;
using System.Linq;

public class SixthSense : Role
{
    protected override List<LayerName> LayersAbleToSee => new()
    {
        LayerName.Mafia,
        LayerName.Guardian,
        LayerName.Seer,
    };
}
