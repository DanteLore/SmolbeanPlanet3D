using System;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        target = transform.position; // We start where we want to be

        var idle = new IdleState(animator);
        var eat = new EatGrassState(this);
        var lookForPlaceToEat = new ChooseEatingPlaceState(this);
        var headToTheWoods = new ChooseWoodlandLocation(this);
        var wander = new WanderState(this, navAgent, animator, soundPlayer);

        AT(wander, HasSomewhereToGo());
        AT(wander, idle, Arrived());

        AT(idle, lookForPlaceToEat, NeedToSearchForFood());
        AT(idle, eat, Hungry());
        AT(idle, headToTheWoods, IdleFor(2f));

        // In case the search states find a very close point
        AT(lookForPlaceToEat, idle, Arrived());
        AT(headToTheWoods, idle, Arrived());

        AT(eat, idle, NotEnoughFoodHere());
        AT(eat, idle, Full());
        
        stateMachine.SetState(idle);

        Func<bool> IdleFor(float seconds) => () => idle.TimeIdle > seconds;
        Func<bool> HasSomewhereToGo() => () => !CloseEnoughTo(target);
        Func<bool> Hungry() => IsHungry;
        Func<bool> Full() => IsFull;
        Func<bool> Arrived() => () => CloseEnoughTo(target);
        Func<bool> NeedToSearchForFood() => () => IsHungry() && !IsEnoughFoodHere();
        Func<bool> NotEnoughFoodHere() => () => !IsEnoughFoodHere();
    }

    private bool IsEnoughFoodHere()
    {
        return GroundWearManager.Instance.GetAvailableGrass(transform.position) >= 0.1f;
    }

    private bool IsFull()
    {
        return foodLevel > species.fullThreshold;
    }

    private bool IsHungry()
    {
        return foodLevel < species.hungryThreshold;
    }

    protected override void Update()
    {
        base.Update();
    }
}
