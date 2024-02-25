using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChooseWoodlandLocation : IState
{
    private readonly SmolbeanAnimal animal;

    public ChooseWoodlandLocation(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        var pos = animal.transform.position;

        var treeLocations = Physics.OverlapSphere(pos, animal.species.sightRange, LayerMask.GetMask("Nature"))
            .Select(c => c.gameObject.GetComponent<SmolbeanTree>())
            .Where(_ => _ != null)
            .Select(tree => tree.transform.position)
            .ToArray();

        float x = 0f, z = 0f;

        foreach(var v in treeLocations)
        {
            x += v.x;
            z += v.z;
        }

        x /= treeLocations.Length;
        z /= treeLocations.Length;

        x += Random.Range(-8, 8);
        z += Random.Range(-8, 8);

        if (
                Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 2.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            animal.target = navHit.position;
        else
            animal.target = pos;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
