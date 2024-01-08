using UnityEngine;
using UnityEngine.UIElements;

public class NavigationControls : MonoBehaviour
{
    public GameObject cameraPositionObject;
    public GridManager gridManager;
    public string groundLayer = "Ground";
    public string seaLayer = "Sea";

    private Label positionLabel;

    private int groundLayerMask;
    
    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        
        groundLayerMask = LayerMask.GetMask(groundLayer, seaLayer);
        positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
    }

    void Update()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, groundLayerMask))
            {
                var pos = gridManager.GetGameSquareFromWorldCoords(hit.point);
                positionLabel.text = $"{pos.x}λ \u00d7 {pos.y}φ";
            }
            else
            {
                positionLabel.text = "";
            }

    }
}
