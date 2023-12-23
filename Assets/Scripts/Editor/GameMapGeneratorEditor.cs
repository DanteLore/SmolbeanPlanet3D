using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameMapGenerator))]
public class GameMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var mapGenerator = (GameMapGenerator)target;

        if(GUILayout.Button("Random Seed"))
        {
            mapGenerator.seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Texture2D texture = new Texture2D(mapGenerator.mapWidth, mapGenerator.mapHeight);

        var map = mapGenerator.GenerateMap(mapGenerator.seed);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = mapGenerator.mapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapGenerator.mapWidth; x++)
            {
                float i = map[y * mapGenerator.mapWidth + x];
                Color color = (i == 0) ? Color.blue : new Color(0f, 1.0f / (i - 1), 0f);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();     

        GUILayout.Label("Preview:");
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUILayout.Label("", GUILayout.Width(400), GUILayout.Height(400));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        GUILayout.EndHorizontal();
    }
}