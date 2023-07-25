using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    public AudioMixer mixer;

    private UIDocument document;
    private string[] files;

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
    }

    private void DoneButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }
}
