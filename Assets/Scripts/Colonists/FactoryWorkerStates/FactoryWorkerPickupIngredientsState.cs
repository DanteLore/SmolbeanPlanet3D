public class FactoryWorkerPickupIngredientsState : IState
{
    private readonly SmolbeanColonist colonist;
    public string Name { get => GetType().Name; }

    public FactoryWorkerPickupIngredientsState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        if (colonist.Job != null)
        {
            var factory = colonist.Job.Building as FactoryBuilding;
            if (factory != null)
                factory.LoadResources();
        }
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
