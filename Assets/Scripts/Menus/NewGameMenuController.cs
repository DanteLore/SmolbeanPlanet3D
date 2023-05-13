using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameMenuController : MonoBehaviour
{
    UIDocument document;
    GameMapGenerator mapGenerator;

    VisualElement previewPane;
    private Toggle randomToggle;
    private TextField seedTextField;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        mapGenerator = FindAnyObjectByType<GameMapGenerator>();
        mapGenerator.OnGameMapChanged += MapChanged;
        
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
        GenerateMap();
    }

    private void CancelButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }

    private void StartGameClicked()
    {
        Debug.Log("Time to start the game!");
    }

    private void NewMapButtonClicked()
    {
        GenerateMap();
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

        mapGenerator.GenerateMap(seed);
    }

    private void MapChanged(object sender, List<int> e)
    {

        Texture2D texture = new Texture2D(mapGenerator.mapWidth, mapGenerator.mapHeight);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = mapGenerator.mapWidth - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapGenerator.mapHeight; x++)
            {
                float i = mapGenerator.GameMap[y * mapGenerator.mapWidth + x];

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
