using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

// Some VERY ugly code in here - but the beautiful and terse linq methods just needed to go, for performance

public class ChooseFlockLocation
    : IState
{
    private readonly SmolbeanAnimal animal;
    private readonly int natureLayer;
    private readonly int groundLayer;
    private readonly int creatureLayer;
    private const int MAX_COLLIDERS = 100;
    private static readonly Collider[] hitColliders = new Collider[MAX_COLLIDERS];

    public ChooseFlockLocation(SmolbeanAnimal animal)
    {
        this.animal = animal;
        natureLayer = LayerMask.GetMask("Nature");
        groundLayer = LayerMask.GetMask("Ground");
        creatureLayer = LayerMask.GetMask("Creatures");
    }

    public void OnEnter()
    {
        var pos = animal.transform.position;

        Vector3 target;
        Vector3 randomOffset = new Vector3(Random.Range(-16f, 16f), 0f, Random.Range(-16f, 16f));
        bool animalTargetFound = TryGetAverageAnimalsLocationAroundPoint(pos, out Vector3 animalTarget);
        bool treeTargetFound = TryGetAverageTreeLocationAroundPoint(pos, out Vector3 treeTarget);

        if(animalTargetFound & treeTargetFound)
        {
            // Follow other animals if we see any
            animal.Think("Following the flock in the woods");
            target = (animalTarget + treeTarget) / 2f + randomOffset;
        }
        else if(animalTargetFound)
        {
            // Otherwise, head for the trees
            animal.Think("Folowing the flock");
            target = animalTarget + randomOffset;
        }
        else if(treeTargetFound)
        {
            // Otherwise, head for the trees
            animal.Think("Heading for the woods");
            target = treeTarget + randomOffset;
        }
        else
        {
            // If all else fails - just go somewhere random
            animal.Think("Searching the island");
            target = randomOffset * 2f;
        }

        if (
                Physics.Raycast(new Ray(new Vector3(target.x, 10000, target.z), Vector3.down), out var rayHit, float.MaxValue, groundLayer)
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 2.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            animal.Target = navHit.position;
        else
            animal.Target = pos;
    }

    private bool TryGetAverageTreeLocationAroundPoint(Vector3 pos, out Vector3 target)
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(pos, animal.Species.sightRange, hitColliders, natureLayer);

        if(colliderCount == 0)
        {
            target = pos;
            return false;
        }

        target = Vector3.zero;
        int count = 0;

        for(int i = 1; i < colliderCount; i++)
        {
            if(hitColliders[i].TryGetComponent<SmolbeanTree>(out var tree))
            {
                target += tree.transformCached.position;
                count++;
            }
        }

        if(count == 0)
        {
            target = pos;
            return false;
        }

        target /= count;

        return true;
    }

    private bool TryGetAverageAnimalsLocationAroundPoint(Vector3 pos, out Vector3 target)
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(pos, animal.Species.sightRange, hitColliders, creatureLayer);

        if(colliderCount == 0)
        {
            target = pos;
            return false;
        }

        target = Vector3.zero;
        int count = 0;

        for(int i = 0; i < colliderCount; i++)
        {
            if(hitColliders[i].transform.parent.TryGetComponent<SmolbeanAnimal>(out var a) && a.SpeciesIndex == animal.SpeciesIndex)
            {
                target += a.transformCached.position;
                count++;
            }
        }

        if(count == 0)
        {
            target = pos;
            return false;
        }

        target /= count;

        return true;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
