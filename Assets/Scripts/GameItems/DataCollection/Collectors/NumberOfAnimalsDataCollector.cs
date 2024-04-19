public class NumberOfAnimalsDataCollector : DataCollectionSeries
{
    public AnimalSpec species;

    private AnimalController animalController;

    private void Start()
    {
        animalController = GetComponent<AnimalController>();
    }

    protected override float GetDataValue()
    {
        return animalController.AnimalCount(species);
    }
}
