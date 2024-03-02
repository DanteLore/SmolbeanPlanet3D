using UnityEngine;

public class ColonistSleepState : IState
{
    private SmolbeanColonist colonist;

    private float sleepStartTime;

    public float TimeAsleep { get { return Time.time - sleepStartTime; } }

    public ColonistSleepState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        sleepStartTime = Time.time;
        colonist.Hide();
    }

    public void OnExit()
    {
        colonist.Show();
    }

    public void Tick()
    {
    }
}
