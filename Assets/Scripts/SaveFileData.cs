using System.Collections.Generic;

public class SaveFileData
{
    public int gameMapWidth;
    public int gameMapHeight;
    public List<int> gameMap;
    public List<NatureObjectSaveData> treeData;
    public List<NatureObjectSaveData> rockData;
    public List<BuildingObjectSaveData> buildingData;
    public List<DropItemSaveData> dropItemData;
    public List<AnimalSaveData> animalData;
    public CameraSaveData cameraData;
    public TimeOfDaySaveData timeData;
    public List<JobSaveData> jobData;
    public WindSaveData windData;
    public List<DataCollectionSeriesSaveData> dataCollectionSeries;
}