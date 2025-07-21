using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SettingsMenuController : SmolbeanMenu
{
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

        var musicVolumeSlider = root.Q<Slider>("musicSlider");
        musicVolumeSlider.value = PrefsManager.Instance.MusicVolume;
        musicVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.MusicVolume = v.newValue;
        });

        var sfxVolumeSlider = root.Q<Slider>("sfxSlider");
        sfxVolumeSlider.value = PrefsManager.Instance.SfxVolume;
        sfxVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.SfxVolume = v.newValue;
        });

        var ambientVolumeSlider = root.Q<Slider>("ambientSlider");
        ambientVolumeSlider.value = PrefsManager.Instance.AmbientVolume;
        ambientVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.AmbientVolume = v.newValue;
        });

        var grassToggle = root.Q<Toggle>("grassToggle");
        grassToggle.value = PrefsManager.Instance.GrassRenderingEnabled;
        grassToggle.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.GrassRenderingEnabled = v.newValue;
        });

        var cloudsToggle = root.Q<Toggle>("cloudsToggle");
        cloudsToggle.value = PrefsManager.Instance.CloudsEnabled;
        cloudsToggle.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.CloudsEnabled = v.newValue;
        });

        var fullscreenToggle = root.Q<Toggle>("fullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        cloudsToggle.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.FullScreen = v.newValue;
        });

        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var resolutions = PrefsManager.Instance.ScreenResolutionOptions.Select(r => $"{r.width}x{r.height}").ToList();
        var res = PrefsManager.Instance.ScreenResolution;
        string currentResolutionString = $"{res.width}x{res.height}";
        resolutionDropdown.choices = resolutions;
        resolutionDropdown.index = resolutions.IndexOf(currentResolutionString);
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
            {
                PrefsManager.Instance.QualityLevel = newLevel;
            }
        });

    }

    private void DoneButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu();
    }
}
