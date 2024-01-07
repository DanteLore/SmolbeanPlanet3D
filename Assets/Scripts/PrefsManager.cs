using UnityEngine;
using UnityEngine.Audio;

public class PrefsManager : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject clouds;

    public static PrefsManager Instance { get; private set; }

    private GrassInstancer grassInstancer; 

    public float MusicVolume
    {
        get 
        { 
            return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        }
        set 
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        }
    }

    public float SfxVolume
    {
        get 
        { 
            return PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        }
        set 
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        }
    }

    public float AmbientVolume
    {
        get 
        { 
            return PlayerPrefs.GetFloat("AmbientVolume", 1.0f);
        }
        set 
        {
            PlayerPrefs.SetFloat("AmbientVolume", value);
            mixer.SetFloat("AmbientVolume", Mathf.Log10(value) * 20);
        }
    }

    public bool GrassRenderingEnabled
    {
        get 
        { 
            return PlayerPrefs.GetInt("GrassRenderingEnabled", 1) == 1;
        }
        set 
        {
            PlayerPrefs.SetInt("GrassRenderingEnabled", value ? 1 : 0);
            grassInstancer.enabled = value;
        }
    }

    public bool CloudsEnabled
    {
        get 
        { 
            return PlayerPrefs.GetInt("CloudsEnabled", 1) == 1;
        }
        set 
        {
            PlayerPrefs.SetInt("CloudsEnabled", value ? 1 : 0);
            clouds.SetActive(value);
        }
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else   
            Instance = this;
    }

    void Start()
    {
        grassInstancer = GameObject.FindFirstObjectByType<GrassInstancer>();
        grassInstancer = GameObject.FindFirstObjectByType<GrassInstancer>();

        MusicVolume = MusicVolume; // Not sure how I feel about this... 😵‍💫
        SfxVolume = SfxVolume;
        AmbientVolume = AmbientVolume;
        GrassRenderingEnabled = GrassRenderingEnabled;
    }
}
