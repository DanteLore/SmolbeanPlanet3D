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
            Resolution res = v.newValue ? Screen.resolutions.Last() : Screen.resolutions.First(r => r.width == 1024);
            Screen.SetResolution(res.width, res.height, v.newValue ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        });

        var resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        var resolutions = Screen.resolutions.Select(r => $"{r.width}x{r.height}").ToList();
        var currentResolutionIndex = Screen.resolutions.ToList().IndexOf(Screen.currentResolution);
        resolutionDropdown.choices = resolutions;
        resolutionDropdown.index = currentResolutionIndex;
        resolutionDropdown.RegisterValueChangedCallback(v =>
        {
            var newRes = Screen.resolutions[resolutionDropdown.choices.IndexOf(v.newValue)];
            Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreenMode);

            fullscreenToggle.value = Screen.fullScreen;
        });

        var qualityDropdown = root.Q<DropdownField>("qualityDropdown");
        var qualityLevels = new List<string>(QualitySettings.names);
        qualityDropdown.choices = qualityLevels;
        var currentQuality = QualitySettings.GetQualityLevel();
        qualityDropdown.value = qualityLevels[currentQuality];
        qualityDropdown.RegisterValueChangedCallback(v =>
        {
            int newLevel = qualityLevels.IndexOf(v.newValue);
            if (newLevel >= 0)
            {
                QualitySettings.SetQualityLevel(newLevel);
            }
        });

    }

    private void DoneButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu();
    }
}
