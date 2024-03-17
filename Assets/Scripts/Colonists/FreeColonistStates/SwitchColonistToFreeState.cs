using UnityEngine;

public class SwitchColonistToFreeState : IState
{
    private readonly SmolbeanColonist colonist;

    public SwitchColonistToFreeState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        colonist.Think("I lost my job!  Guess I'm a free agent again.");

        AnimalController.Instance.SwitchAnimal(colonist, JobController.Instance.freeColonistPrefab);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
