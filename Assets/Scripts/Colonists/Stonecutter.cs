using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class Stonecutter : ResourceGatherer
{
    protected override Type GetTargetType()
    {
        return typeof(SmolbeanRock);
    }
    
    protected override string GetGatheringTrigger()
    {
        return "StartMining";
    }
}
