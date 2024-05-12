using System;

public class Woodcutter : StaticResourceGatherer
{
    protected override Type GetTargetType()
    {
        return typeof(SmolbeanTree);
    }
    
    protected override string GetGatheringTrigger()
    {
        return "StartChopping";
    }
}
