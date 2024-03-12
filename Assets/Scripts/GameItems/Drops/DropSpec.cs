using UnityEngine;

[CreateAssetMenu(fileName = "DropSpec", menuName = "Smolbean/Drop Spec", order = 1)]
public class DropSpec : ScriptableObject
{
    public string dropName;

    public int dropRate = 3;
    public int stackSize = 9;

    public Texture2D thumbnail;
    public GameObject singlePrefab;
    public GameObject somePrefab;
    public GameObject lotsPrefab;
    public int lifeSpanSeconds;

    public GameObject GetPrefabFor(int qtty)
    {
        if(qtty <= stackSize / 3)
            return singlePrefab;
        else if (qtty <= (stackSize * 2) / 3)
            return somePrefab;
        else
            return lotsPrefab;
    }
}
