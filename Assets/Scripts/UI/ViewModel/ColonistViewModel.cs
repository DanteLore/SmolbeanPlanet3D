
using UnityEngine;

[System.Serializable]
public class ColonistViewModel
{
    private SmolbeanColonist colonist;

    public ColonistViewModel(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public string ColonistName
    {
        get { return colonist.Stats.name; }
    }

    public Texture2D ColonistThumbnail
    {
        get { return colonist.Species.thumbnail; }
    }
}
