using System;

public class Woodcutter : ResourceGatherer
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
