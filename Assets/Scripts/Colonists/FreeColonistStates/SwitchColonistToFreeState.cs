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

        colonist.gameObject.SetActive(false);

        Vector3 pos = colonist.transform.position;
        Quaternion rot = colonist.transform.rotation;
        Transform parent = colonist.transform.parent;

        DropInventory(pos);

        var newColonist = Object.Instantiate(JobController.Instance.freeColonistPrefab, pos, rot, parent);
        newColonist.GetComponent<SmolbeanColonist>().AdoptIdentity(colonist);

        Object.Destroy(colonist.gameObject);
    }

    private void DropInventory(Vector3 pos)
    {
        while (!colonist.Inventory.IsEmpty())
        {
            var item = colonist.Inventory.DropLast();

            Vector3 upPos = Vector3.up * 1f;
            Vector3 outPos = Vector3.left * Random.Range(0f, 1f);
            outPos = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * outPos;

            DropController.Instance.Drop(item.dropSpec, pos + upPos + outPos, item.quantity);
        }
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
