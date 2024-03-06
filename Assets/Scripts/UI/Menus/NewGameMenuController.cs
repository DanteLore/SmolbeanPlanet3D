using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameMenuController : SmolbeanMenu
{
    private UIDocument document;
    private SoundPlayer soundPlayer;
    private GameMapCreator mapCreator;
    private MapGeneratorManager mapGeneratorManager;
    private VisualElement previewPane;
    private TextField seedTextField;
    private List<int> map;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
        mapCreator = FindFirstObjectByType<GameMapCreator>();
        mapGeneratorManager = FindFirstObjectByType<MapGeneratorManager>();
        
        var startGameButton = document.rootVisualElement.Q<Button>("startGameButton");
        startGameButton.clicked += StartGameClicked;

        var randomButton = document.rootVisualElement.Q<Button>("randomButton");
        randomButton.clicked += RandomButtonClicked;

        var slider = document.rootVisualElement.Q<Slider>("noiseScale1Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseScale1 = v.newValue; SettingChanged(); });
        var range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseScale1)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseScale1;

        slider = document.rootVisualElement.Q<Slider>("noiseScale2Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseScale2 = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseScale2)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseScale2;

        slider = document.rootVisualElement.Q<Slider>("noiseScale3Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseScale3 = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseScale3)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseScale3;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength1Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseStength1 = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseStength1)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseStength1;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength2Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseStength2 = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseStength2)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseStength2;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength3Slider");
        slider.RegisterValueChangedCallback(v => { mapCreator.noiseStength3 = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.noiseStength3)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.noiseStength3;

        slider = document.rootVisualElement.Q<Slider>("heightAdjustSlider");
        slider.RegisterValueChangedCallback(v => { mapCreator.heightBias = v.newValue; SettingChanged(); });
        range = typeof(GameMapCreator).GetField(nameof(GameMapCreator.heightBias)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapCreator.heightBias;

        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;

        seedTextField = document.rootVisualElement.Q<TextField>("seedTextField");
        seedTextField.RegisterValueChangedCallback(v => { SettingChanged(); });
        seedTextField.value = mapCreator.seed.ToString();

        previewPane = document.rootVisualElement.Q<VisualElement>("newMapPreview");
        
        map = mapGeneratorManager.GameMap;
        DrawMap();
    }

    private void SettingChanged()
    {
        GenerateMap();
        DrawMap();
    }

    private void CancelButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu();
    }

    private void StartGameClicked()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        soundPlayer.Play("Click");
        yield return null;
        document.rootVisualElement.style.display = DisplayStyle.None;
        // Show a please wait message here
        yield return StartCoroutine(mapGeneratorManager.Recreate(map, mapCreator.mapWidth, mapCreator.mapHeight));
        // Hide the please wait message here
        MenuController.Instance.CloseAll();
    }

    private void RandomButtonClicked()
    {
        soundPlayer.Play("Click");
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        seedTextField.value = seed.ToString();
    }

    private void GenerateMap()
    {
        if (int.TryParse(seedTextField.value, out int seed))
        {
            map = mapCreator.GenerateMap(seed);
        }
    }

    private void DrawMap()
    {
        Texture2D texture = new(mapCreator.mapWidth, mapCreator.mapHeight);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = mapCreator.mapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapCreator.mapWidth; x++)
            {
                int i = map[y * mapCreator.mapWidth + x];
                float g = (mapCreator.maxLevelNumber + 1.0f - i) / (mapCreator.maxLevelNumber + 1.0f);
                Color color = (i == 0) ? Color.blue : new Color(0f, Mathf.Lerp(0.1f, 1.0f, g), 0f);
                texture.SetPixel(x, y, color);
            }
        }

        previewPane.style.backgroundImage = texture;

        texture.Apply();
    }
}
