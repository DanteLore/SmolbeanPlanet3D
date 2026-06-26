using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class PrefsManager : MonoBehaviour
{
    // Inspector fields
    public AudioMixer mixer;
    public GameObject clouds;
    public GrassInstancer grassInstancer;

    // Singleton
    public static PrefsManager Instance { get; private set; }

    // Camera speed cache
    private float? _panSpeed;
    private float? _zoomSpeed;
    private float? _rotateSpeed;
    private float? _altitudeSpeedMultiplier;

    public void InvalidateCache()
    {
        _panSpeed = null;
        _zoomSpeed = null;
        _rotateSpeed = null;
        _altitudeSpeedMultiplier = null;
    }

    // Camera speed properties
    public float PanSpeed
    {
        get => _panSpeed ??= PlayerPrefs.GetFloat("PanSpeed", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("PanSpeed", value);
            _panSpeed = value;
        }
    }

    public float ZoomSpeed
    {
        get => _zoomSpeed ??= PlayerPrefs.GetFloat("ZoomSpeed", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("ZoomSpeed", value);
            _zoomSpeed = value;
        }
    }

    public float RotateSpeed
    {
        get => _rotateSpeed ??= PlayerPrefs.GetFloat("RotateSpeed", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("RotateSpeed", value);
            _rotateSpeed = value;
        }
    }

    public float AltitudeSpeedMultiplier
    {
        get => _altitudeSpeedMultiplier ??= PlayerPrefs.GetFloat("AltitudeSpeedMultiplier", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("AltitudeSpeedMultiplier", value);
            _altitudeSpeedMultiplier = value;
        }
    }

    // Sound properties
    public float MusicVolume
    {
        get => PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        }
    }

    public float SfxVolume
    {
        get => PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        }
    }

    public float AmbientVolume
    {
        get => PlayerPrefs.GetFloat("AmbientVolume", 1.0f);
        set
        {
            PlayerPrefs.SetFloat("AmbientVolume", value);
            mixer.SetFloat("AmbientVolume", Mathf.Log10(value) * 20);
        }
    }

    // Graphics properties
    public bool GrassRenderingEnabled
    {
        get => PlayerPrefs.GetInt("GrassRenderingEnabled", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("GrassRenderingEnabled", value ? 1 : 0);
            grassInstancer.enabled = value;
        }
    }

    public bool CloudsEnabled
    {
        get => PlayerPrefs.GetInt("CloudsEnabled", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("CloudsEnabled", value ? 1 : 0);
            clouds.SetActive(value);
        }
    }

    public List<string> QualityLevelChoices => QualitySettings.names.ToList();

    public int QualityLevel
    {
        get => PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        set
        {
            PlayerPrefs.SetInt("QualityLevel", value);
            QualitySettings.SetQualityLevel(value);
        }
    }

    // Screen properties
    public string LastSaveName
    {
        get => PlayerPrefs.GetString("LastSaveName", null);
        set => PlayerPrefs.SetString("LastSaveName", value);
    }

    public List<Resolution> ScreenResolutionOptions => Screen.resolutions.ToList();

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

    private void SetScreenProperties()
    {
        Screen.SetResolution(ScreenResolution.width, ScreenResolution.height, FullScreen);
        Screen.fullScreen = FullScreen;
    }

    // Unity lifecycle
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        grassInstancer = FindAnyObjectByType<GrassInstancer>();

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
