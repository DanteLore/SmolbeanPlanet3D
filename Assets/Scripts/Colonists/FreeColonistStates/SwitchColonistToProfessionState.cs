using UnityEngine;

public class SwitchColonistToProfessionState : IState
{
    private readonly SmolbeanColonist colonist;

    public SwitchColonistToProfessionState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        colonist.gameObject.SetActive(false);

        JobSpec spec = colonist.job.JobSpec;
        Vector3 pos = colonist.transform.position;
        Quaternion rot = colonist.transform.rotation;
        Transform parent = colonist.transform.parent;

        var newColonist = Object.Instantiate(spec.colonistPrefab, pos, rot, parent);
        newColonist.GetComponent<SmolbeanColonist>().AdoptIdentity(colonist);

        Object.Destroy(colonist.gameObject);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
