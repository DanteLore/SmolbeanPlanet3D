using UnityEngine;

public class ShootState : IState
{
    Hunter hunter;

    public ShootState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        hunter.Shoot();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
