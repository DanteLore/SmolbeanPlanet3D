using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    public string defaultToolbarName = "MainToolbar";
    private SoundPlayer soundPlayer;

    public static ToolbarController Instance { get; private set; }

    private bool isVisible;

    void Awake()
    {
        if(Instance != null && Instance != this)
            DestroyImmediate(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        isVisible = false; // force this to true when the game starts to stop a sound playing :)
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
        CloseAll();
    }
    
    public void ShowToolbar(string name = null)
    {
        if(name == null)
            name = defaultToolbarName;

        foreach(Transform child in transform)
        {
            if (child.gameObject.name == name)
            {
                if (isVisible == true) // Only play a sound if we're switching toolbars
                    soundPlayer.Play("Whoosh2");
                child.gameObject.SetActive(true);
                isVisible = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void CloseAll()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        isVisible = false;
    }
}
