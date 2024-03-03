using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MapMenuController : SmolbeanMenu
{
    public Texture2D groundTexture;
    public Material mapMaterial;
    public int natureLayerResolution = 256;
    public Color treeColor;
    public Color rockColor;
    public Color seaColor;
    public GridManager gridManager;
    public GameObject trees;
    public GameObject rocks;

    private UIDocument document;
    private SoundPlayer soundPlayer;
    private VisualElement mapBox;
    private Texture2D mapTexture;
    private float width;
    private float height;
    private float xOffset;
    private float zOffset;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
        mapBox = document.rootVisualElement.Q<VisualElement>("mapBox");
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += () =>
        {
            soundPlayer.Play("Click");
            MenuController.Instance.CloseAll();
        };

        mapTexture = new Texture2D(groundTexture.width, groundTexture.height) { filterMode = FilterMode.Point };

        mapMaterial.SetTexture("_mapTexture", DrawMap());

        width = gridManager.tileSize * gridManager.DrawMapWidth;
        height = gridManager.tileSize * gridManager.DrawMapHeight;
        xOffset = (width + gridManager.tileSize) / 2f;
        zOffset = (height + gridManager.tileSize) / 2f;

        InvokeRepeating("GenerateMapTexture", 0.1f, 5.0f);
    }

    void OnDisable()
    {
        CancelInvoke("GenerateMapTexture");
    }

    private void GenerateMapTexture()
    {
        mapMaterial.SetTexture("_natureTexture", DrawNature());

        RenderTexture r = new RenderTexture(groundTexture.width, groundTexture.height, 16);
        Graphics.Blit(groundTexture, r, mapMaterial);
        RenderTexture.active = r;
        mapTexture.ReadPixels(new Rect(0, 0, r.width, r.height), 0, 0);
        mapTexture.Apply();

        mapBox.style.backgroundImage = mapTexture;
    }

    private Texture2D DrawMap()
    {
        var tex = new Texture2D(gridManager.GameMapWidth, gridManager.GameMapWidth);

        // Map
        for(int y = 0; y < gridManager.GameMapWidth; y++)
        {
            for(int x = 0; x < gridManager.GameMapWidth; x++) 
            {
                int val = gridManager.GameMap[y * gridManager.GameMapWidth + x];
                float g = (gridManager.MaxLevelNumber + 1.0f - val) / (gridManager.MaxLevelNumber + 1.0f);
                Color c = (val == 0) ? seaColor : new Color(0f, g, 0f);
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        
        return tex;
    }

    private Texture2D DrawNature()
    {
        var tex = new Texture2D(natureLayerResolution, natureLayerResolution);
        var pixels = Enumerable.Repeat(new Color(0f, 0f, 0f, 0f), natureLayerResolution * natureLayerResolution).ToArray();
        tex.SetPixels(pixels);

        foreach(var rock in rocks.GetComponentsInChildren<SmolbeanRock>())
        {
            int x = Mathf.FloorToInt((rock.transform.position.x + xOffset) * natureLayerResolution / width);
            int y = Mathf.FloorToInt((rock.transform.position.z + zOffset) * natureLayerResolution / height);

            tex.SetPixel(x, y, rockColor);
        }

        foreach(var tree in trees.GetComponentsInChildren<SmolbeanTree>())
        {
            int x = Mathf.FloorToInt((tree.transform.position.x + xOffset) * natureLayerResolution / width);
            int y = Mathf.FloorToInt((tree.transform.position.z + zOffset) * natureLayerResolution / height);

            tex.SetPixel(x, y, treeColor);
        }

        tex.Apply();
        
        return tex;
    }
}
