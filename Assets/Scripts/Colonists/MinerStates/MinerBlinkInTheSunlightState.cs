public class MinerBlinkInTheSunlightState : IState
{
    private readonly Miner miner;

    public MinerBlinkInTheSunlightState(Miner miner)
    {
        this.miner = miner;
    }

    public void OnEnter()
    {
        miner.Think("Nice to see the outside world!");
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }
}
