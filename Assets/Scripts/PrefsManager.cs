using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class PrefsManager : MonoBehaviour
{
    public AudioMixer mixer;
    public GameObject clouds;
    public GrassInstancer grassInstancer;

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

    public string LastSaveName
    {
        get 
        { 
            return PlayerPrefs.GetString("LastSaveName", null);
        }
        set 
        {
            PlayerPrefs.SetString("LastSaveName", value);
        }
    }

    public List<Resolution> ScreenResolutionOptions
    {
        get => Screen.resolutions.ToList();
    }

    public bool FullScreen
    {
        get => PlayerPrefs.GetInt("FullScreen", Screen.fullScreen ? 1 : 0) == 1;

        set
        {
            PlayerPrefs.SetInt("FullScreen", value ? 1 : 0);
            SetScreenProperties();
        }
    }

    public Resolution ScreenResolution
    {
        get => new Resolution
        {
            width = PlayerPrefs.GetInt("ScreenResolutionWidth", Screen.currentResolution.width),
            height = PlayerPrefs.GetInt("ScreenResolutionHeight", Screen.currentResolution.height)
        };

        set
        {
            PlayerPrefs.SetInt("ScreenResolutionWidth", Screen.currentResolution.width);
            PlayerPrefs.SetInt("ScreenResolutionHeight", Screen.currentResolution.height);
            SetScreenProperties();
        }
    }

    public List<string> QualityLevelChoices
    {
        get => QualitySettings.names.ToList();
    }

    public int QualityLevel
    {
        get => PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());

        set
        {
            PlayerPrefs.SetInt("QualityLevel", value);
            QualitySettings.SetQualityLevel(value);
        }
    }

    private void SetScreenProperties()
    {
        Screen.SetResolution(ScreenResolution.width, ScreenResolution.height, FullScreen);
        Screen.fullScreen = FullScreen;
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
        grassInstancer = FindFirstObjectByType<GrassInstancer>();

        MusicVolume = MusicVolume; // Not sure how I feel about this... 😵‍💫
        SfxVolume = SfxVolume;     // BUT
        AmbientVolume = AmbientVolume; // It's the easiest way to apply the settings
        GrassRenderingEnabled = GrassRenderingEnabled; // Sorry!
        FullScreen = FullScreen;
        ScreenResolution = ScreenResolution;
    }

    public void SavePrefs()
    {
        PlayerPrefs.Save();
    }
}
