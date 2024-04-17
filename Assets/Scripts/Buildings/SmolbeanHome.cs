using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmolbeanHome : MonoBehaviour
{
    public int colonistsToSpawn = 4;
    public int maxCapacity = 4;
    public bool unlimitedCapacity = false;
    public AnimalSpec colonistSpec;
    public string creatureLayer = "Creatures";
    public float initialSpawnDelay = 2f;
    public float intermediateSpawnDelay = 30f;
    public ParticleSystem birthParticleSystem;

    private SmolbeanBuilding building;
    private readonly List<SmolbeanColonist> colonists = new();
    public string BuildingName { get { return building.name; }}

    public IEnumerable<SmolbeanColonist> Colonists { get { return colonists; } }

    public bool HasVacancy { get { return unlimitedCapacity || colonists.Count < maxCapacity; }}

    private void Start()
    {
        building = GetComponent<SmolbeanBuilding>();
        Debug.Assert(building != null, "A home must be a building!");

        StartCoroutine(nameof(CreateColonists));
    }

    private void OnDestroy()
    {    
        if(!GameStateManager.Instance.IsStarted)
            return; // In the case where the map itself is being destroyed - e.g. new game or quit

        while(colonists.Count > 0)  
        {   
            var colonist = colonists[0];
            var newHome = FindNewHome();
            colonist.Home = newHome;
            newHome.AddColonist(colonist);
            colonists.Remove(colonist);
        }
    }

    private SmolbeanHome FindNewHome()
    {
        var newHome = BuildingController.Instance.GetAllHomes()
            .Where(h => h.HasVacancy)
            .Where(h => h.GetComponent<Shipwreck>() == null) // Try houses first
            .OrderBy(h => Vector3.SqrMagnitude(h.transform.position - transform.position))
            .FirstOrDefault();

        // Default to shipwreck if no other homes found
        return newHome != null ? newHome : ShipwreckManager.Instance.Shipwreck.GetComponent<SmolbeanHome>();
    }

    private IEnumerator CreateColonists()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while(colonists.Count < colonistsToSpawn)
        {
            yield return new WaitUntil(SpawnPointEmpty);

            var colonist = (SmolbeanColonist)AnimalController.Instance.CreateAnimal(colonistSpec, building.dropPoint.transform.position);
            colonist.Home = this;
            colonists.Add(colonist);

            colonist.GetComponent<SoundPlayer>().Play("Appear");
            
            yield return new WaitForSeconds(intermediateSpawnDelay);
        }
    }

    private bool SpawnPointEmpty()
    {
        return !Physics.CheckSphere(building.dropPoint.transform.position, 2f, LayerMask.GetMask(creatureLayer));
    }

    public void SwapColonist(SmolbeanColonist originalColonist, SmolbeanColonist newColonist)
    {
        colonists.Remove(originalColonist);
        colonists.Add(newColonist);
    }

    public void AddColonist(SmolbeanColonist colonist)
    {
        if(HasVacancy)
            colonists.Add(colonist);
        else
            Debug.LogError($"Can not add additional colonist {colonist.Stats.name} to {building.name} because it's full");
    }
}
