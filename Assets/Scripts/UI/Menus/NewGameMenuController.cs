using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        mapGenerator = FindObjectOfType<GameMapGenerator>();
        gridManager = FindObjectOfType<GridManager>();
        
        var startGameButton = document.rootVisualElement.Q<Button>("startGameButton");
        startGameButton.clicked += StartGameClicked;

        var randomButton = document.rootVisualElement.Q<Button>("randomButton");
        randomButton.clicked += RandomButtonClicked;

        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;

        seedTextField = document.rootVisualElement.Q<TextField>("seedTextField");
        seedTextField.RegisterCallback<ChangeEvent<string>>(TextChanged);
        seedTextField.value = mapGenerator.seed.ToString();

        previewPane = document.rootVisualElement.Q<VisualElement>("newMapPreview");
        
        map = gridManager.GameMap;
        DrawMap();
    }

    private void TextChanged(ChangeEvent<string> evt)
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
                float i = map[y * mapGenerator.mapWidth + x];

                Color color = Color.blue;

                color = (i == 0) ? Color.blue : new Color(0f, 1.0f / (i - 1), 0f);
                texture.SetPixel(x, y, color);
            }
        }

        previewPane.style.backgroundImage = texture;

        texture.Apply();
    }
}
