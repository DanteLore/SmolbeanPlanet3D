using System;

public class Stonecutter : StaticResourceGatherer
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
