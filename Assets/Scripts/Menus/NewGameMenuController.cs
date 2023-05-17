using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameMenuController : MonoBehaviour
{
    UIDocument document;
    GameMapGenerator mapGenerator;
    GridManager gridManager;
    TreeGenerator treeGenerator;
    RockGenerator rockGenerator;
    private GrassInstancer grassInstancer;
    VisualElement previewPane;
    private Toggle randomToggle;
    private TextField seedTextField;
    private List<int> map;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        mapGenerator = FindObjectOfType<GameMapGenerator>();
        gridManager = FindObjectOfType<GridManager>();
        
        var startGameButton = document.rootVisualElement.Q<Button>("startGameButton");
        startGameButton.clicked += StartGameClicked;

        var newMapButton = document.rootVisualElement.Q<Button>("newMapButton");
        newMapButton.clicked += NewMapButtonClicked;

        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;

        randomToggle = document.rootVisualElement.Q<Toggle>("randomToggle");
        randomToggle.RegisterValueChangedCallback(OnRandomToggleChanged);
        seedTextField = document.rootVisualElement.Q<TextField>("seedTextField");
        previewPane = document.rootVisualElement.Q<VisualElement>("newMapPreview");

        randomToggle.value = true;
        seedTextField.SetEnabled(false);
        
        map = gridManager.GameMap;
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

    private void NewMapButtonClicked()
    {
        GenerateMap();
        DrawMap();
    }

    private void GenerateMap()
    {
        int seed = 0;
        if (!randomToggle.value && int.TryParse(seedTextField.value, out int val))
        {
            seed = val;
        }
        else
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        seedTextField.value = seed.ToString();

        map = mapGenerator.GenerateMap(seed);
    }

    private void DrawMap()
    {
        Texture2D texture = new Texture2D(mapGenerator.mapWidth, mapGenerator.mapHeight);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = mapGenerator.mapWidth - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapGenerator.mapHeight; x++)
            {
                float i = map[y * mapGenerator.mapWidth + x];

                Color color = Color.blue;

                if (i == 2)
                {
                    color = new Color(0f, 1.0f, 0f);
                }
                else if (i == 1)
                {
                    color = new Color(0f, 0.75f, 0f);
                }

                texture.SetPixel(x, y, color);
            }
        }

        previewPane.style.backgroundImage = texture;

        texture.Apply();
    }

    private void OnRandomToggleChanged(ChangeEvent<bool> evt)
    {
        if(randomToggle.value)
        {
            seedTextField.SetEnabled(false);
        }
        else
        {
            seedTextField.SetEnabled(true);
        }
    }
}
