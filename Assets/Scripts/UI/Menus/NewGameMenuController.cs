using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameMenuController : SmolbeanMenu
{
    UIDocument document;
    GameMapGenerator mapGenerator;
    GridManager gridManager;
    VisualElement previewPane;
    private TextField seedTextField;
    private List<int> map;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        mapGenerator = FindFirstObjectByType<GameMapGenerator>();
        gridManager = FindFirstObjectByType<GridManager>();
        
        var startGameButton = document.rootVisualElement.Q<Button>("startGameButton");
        startGameButton.clicked += StartGameClicked;

        var randomButton = document.rootVisualElement.Q<Button>("randomButton");
        randomButton.clicked += RandomButtonClicked;

        var slider = document.rootVisualElement.Q<Slider>("noiseScale1Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseScale1 = v.newValue; SettingChanged(); });
        var range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseScale1)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseScale1;

        slider = document.rootVisualElement.Q<Slider>("noiseScale2Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseScale2 = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseScale2)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseScale2;

        slider = document.rootVisualElement.Q<Slider>("noiseScale3Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseScale3 = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseScale3)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseScale3;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength1Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseStength1 = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseStength1)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseStength1;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength2Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseStength2 = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseStength2)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseStength2;

        slider = document.rootVisualElement.Q<Slider>("noiseStrength3Slider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.noiseStength3 = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.noiseStength3)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.noiseStength3;

        slider = document.rootVisualElement.Q<Slider>("heightAdjustSlider");
        slider.RegisterValueChangedCallback(v => { mapGenerator.heightBias = v.newValue; SettingChanged(); });
        range = typeof(GameMapGenerator).GetField(nameof(GameMapGenerator.heightBias)).GetCustomAttribute<RangeAttribute>();
        slider.lowValue = range.min;
        slider.highValue = range.max;
        slider.value = mapGenerator.heightBias;

        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;

        seedTextField = document.rootVisualElement.Q<TextField>("seedTextField");
        seedTextField.RegisterValueChangedCallback(v => { SettingChanged(); });
        seedTextField.value = mapGenerator.seed.ToString();

        previewPane = document.rootVisualElement.Q<VisualElement>("newMapPreview");
        
        map = gridManager.GameMap;
        DrawMap();
    }

    private void SettingChanged()
    {
        GenerateMap();
        DrawMap();
    }

    private void CancelButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }

    private void StartGameClicked()
    {
        gridManager.Recreate(map, mapGenerator.mapWidth, mapGenerator.mapHeight);
        MenuController.Instance.CloseAll();
    }

    private void RandomButtonClicked()
    {
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        seedTextField.value = seed.ToString();
    }

    private void GenerateMap()
    {
        if (int.TryParse(seedTextField.value, out int seed))
        {
            map = mapGenerator.GenerateMap(seed);
        }
    }

    private void DrawMap()
    {
        Texture2D texture = new Texture2D(mapGenerator.mapWidth, mapGenerator.mapHeight);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = mapGenerator.mapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapGenerator.mapWidth; x++)
            {
                int i = map[y * mapGenerator.mapWidth + x];
                float g = (mapGenerator.maxLevelNumber + 1.0f - i) / (mapGenerator.maxLevelNumber + 1.0f);
                Color color = (i == 0) ? Color.blue : new Color(0f, Mathf.Lerp(0.1f, 1.0f, g), 0f);
                texture.SetPixel(x, y, color);
            }
        }

        previewPane.style.backgroundImage = texture;

        texture.Apply();
    }
}
