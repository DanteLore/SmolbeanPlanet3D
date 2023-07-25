using UnityEngine;
using UnityEngine.Audio;

public class PrefsManager : MonoBehaviour
{
    public AudioMixer mixer;

    public static PrefsManager Instance { get; private set; }

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

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else   
            Instance = this;
    }

    void Start()
    {
        MusicVolume = MusicVolume; // Not sure how I feel about this... 😵‍💫
        SfxVolume = SfxVolume;
        AmbientVolume = AmbientVolume;
    }
}
