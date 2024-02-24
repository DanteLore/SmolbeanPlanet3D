public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var eat = new EatGrassState(this);

    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
