using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsMenuController : SmolbeanMenu
{
    private const float CameraSpeedMin = 0.1f;
    private const float CameraSpeedMax = 3f;


    public AudioMixer mixer;

    private UIDocument document;
    private SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var doneButton = root.Q<Button>("doneButton");
        doneButton.clicked += DoneButtonClicked;

        SetupSoundTab(root);
        SetupGraphicsTab(root);
        SetupCameraTab(root);
    }

    private void SetupSoundTab(VisualElement root)
    {
        var musicVolumeSlider = root.Q<Slider>("musicSlider");
        musicVolumeSlider.value = PrefsManager.Instance.MusicVolume;
        musicVolumeSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.MusicVolume = v.newValue);

        var sfxVolumeSlider = root.Q<Slider>("sfxSlider");
        sfxVolumeSlider.value = PrefsManager.Instance.SfxVolume;
        sfxVolumeSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.SfxVolume = v.newValue);

        var ambientVolumeSlider = root.Q<Slider>("ambientSlider");
        ambientVolumeSlider.value = PrefsManager.Instance.AmbientVolume;
        ambientVolumeSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.AmbientVolume = v.newValue);
    }

    private void SetupGraphicsTab(VisualElement root)
    {
        var grassToggle = root.Q<Toggle>("grassToggle");
        grassToggle.value = PrefsManager.Instance.GrassRenderingEnabled;
        grassToggle.RegisterValueChangedCallback(v => PrefsManager.Instance.GrassRenderingEnabled = v.newValue);

        var cloudsToggle = root.Q<Toggle>("cloudsToggle");
        cloudsToggle.value = PrefsManager.Instance.CloudsEnabled;
        cloudsToggle.RegisterValueChangedCallback(v => PrefsManager.Instance.CloudsEnabled = v.newValue);

        var fullscreenToggle = root.Q<Toggle>("fullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        fullscreenToggle.RegisterValueChangedCallback(v => PrefsManager.Instance.FullScreen = v.newValue);

        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var resolutions = PrefsManager.Instance.ScreenResolutionOptions.Select(r => $"{r.width}x{r.height}").ToList();
        var res = PrefsManager.Instance.ScreenResolution;
        resolutionDropdown.choices = resolutions;
        resolutionDropdown.index = resolutions.IndexOf($"{res.width}x{res.height}");
        resolutionDropdown.RegisterValueChangedCallback(v =>
        {
            var index = resolutionDropdown.choices.IndexOf(v.newValue);
            PrefsManager.Instance.ScreenResolution = PrefsManager.Instance.ScreenResolutionOptions[index];
        });

        var qualityDropdown = root.Q<DropdownField>("qualityDropdown");
        var qualityLevels = PrefsManager.Instance.QualityLevelChoices;
        qualityDropdown.choices = qualityLevels;
        qualityDropdown.value = qualityLevels[PrefsManager.Instance.QualityLevel];
        qualityDropdown.RegisterValueChangedCallback(v =>
        {
            int newLevel = qualityLevels.IndexOf(v.newValue);
            if (newLevel >= 0)
                PrefsManager.Instance.QualityLevel = newLevel;
        });
    }

    private void SetupCameraTab(VisualElement root)
    {
        var panSpeedSlider = root.Q<Slider>("panSpeedSlider");
        panSpeedSlider.lowValue = CameraSpeedMin;
        panSpeedSlider.highValue = CameraSpeedMax;
        panSpeedSlider.value = PrefsManager.Instance.PanSpeed;
        panSpeedSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.PanSpeed = v.newValue);

        var zoomSpeedSlider = root.Q<Slider>("zoomSpeedSlider");
        zoomSpeedSlider.lowValue = CameraSpeedMin;
        zoomSpeedSlider.highValue = CameraSpeedMax;
        zoomSpeedSlider.value = PrefsManager.Instance.ZoomSpeed;
        zoomSpeedSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.ZoomSpeed = v.newValue);

        var rotateSpeedSlider = root.Q<Slider>("rotateSpeedSlider");
        rotateSpeedSlider.lowValue = CameraSpeedMin;
        rotateSpeedSlider.highValue = CameraSpeedMax;
        rotateSpeedSlider.value = PrefsManager.Instance.RotateSpeed;
        rotateSpeedSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.RotateSpeed = v.newValue);

        var altitudeSpeedSlider = root.Q<Slider>("altitudeSpeedSlider");
        altitudeSpeedSlider.lowValue = CameraSpeedMin;
        altitudeSpeedSlider.highValue = CameraSpeedMax;
        altitudeSpeedSlider.value = PrefsManager.Instance.AltitudeSpeedMultiplier;
        altitudeSpeedSlider.RegisterValueChangedCallback(v => PrefsManager.Instance.AltitudeSpeedMultiplier = v.newValue);
    }

    private void DoneButtonClicked()
    {
        soundPlayer.Play("Click");
        PrefsManager.Instance.InvalidateCache();
        MenuController.Instance.ShowMenu();
    }
}
