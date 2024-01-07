using UnityEngine.UIElements;
using UnityEngine.Audio;
using UnityEngine;
using System.Linq;

public class SettingsMenuController : SmolbeanMenu
{
    public AudioMixer mixer;
    private UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var doneButton = document.rootVisualElement.Q<Button>("doneButton");
        doneButton.clicked += DoneButtonClicked;

        var musicVolumeSlider = document.rootVisualElement.Q<Slider>("musicSlider");
        musicVolumeSlider.value = PrefsManager.Instance.MusicVolume;
        musicVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.MusicVolume = v.newValue;
        });

        var sfxVolumeSlider = document.rootVisualElement.Q<Slider>("sfxSlider");
        sfxVolumeSlider.value = PrefsManager.Instance.SfxVolume;
        sfxVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.SfxVolume = v.newValue;
        });

        var ambientVolumeSlider = document.rootVisualElement.Q<Slider>("ambientSlider");
        ambientVolumeSlider.value = PrefsManager.Instance.AmbientVolume;
        ambientVolumeSlider.RegisterValueChangedCallback(v =>
        {
            PrefsManager.Instance.AmbientVolume = v.newValue;
        });

        var grassToggle = document.rootVisualElement.Q<Toggle>("grassToggle");
        grassToggle.value = PrefsManager.Instance.GrassRenderingEnabled;
        grassToggle.RegisterValueChangedCallback(v => 
        {
            PrefsManager.Instance.GrassRenderingEnabled = v.newValue;
        });

        var cloudsToggle = document.rootVisualElement.Q<Toggle>("cloudsToggle");
        cloudsToggle.value = PrefsManager.Instance.CloudsEnabled;
        cloudsToggle.RegisterValueChangedCallback(v => 
        {
            PrefsManager.Instance.CloudsEnabled = v.newValue;
        });

        var fullscreenToggle = document.rootVisualElement.Q<Toggle>("fullscreenToggle");
        fullscreenToggle.value = Screen.fullScreen;
        cloudsToggle.RegisterValueChangedCallback(v => 
        {
            Resolution res = v.newValue ? Screen.resolutions.Last() : Screen.resolutions.First(r => r.width == 1024);
            Screen.SetResolution(res.width, res.height, v.newValue ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        });

        var resolutionDropdown = document.rootVisualElement.Q<DropdownField>("resolutionDropdown");
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
    }

    private void DoneButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }
}
