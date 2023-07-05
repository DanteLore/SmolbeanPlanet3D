using UnityEngine;

public class Porter : Colonist
{    
    public float idleTime = 1f;
    public float sleepTime = 2f;

    public GameObject DropTarget { get; set; }

    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine();

        var idle = new IdleState(animator);

        stateMachine.SetState(idle);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
