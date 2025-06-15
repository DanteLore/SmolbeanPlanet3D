using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationControls : MonoBehaviour
{
    public GridManager gridManager;
    public string groundLayer = "Ground";
    public string seaLayer = "Sea";
    public float rotationOffset = -48f;
    public Camera mainCamera;

    private Label positionLabel;
    private Label directionLabel;

    private int groundLayerMask;

    private static readonly StringBuilder stringBuilder = new (32);
    
    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        
        groundLayerMask = LayerMask.GetMask(groundLayer, seaLayer);
        positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        directionLabel = document.rootVisualElement.Q<Label>("directionLabel");
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, groundLayerMask))
        {
            var pos = gridManager.GetGameSquareFromWorldCoords(hit.point);
            stringBuilder.Clear();
            stringBuilder.Append(pos.x);
            stringBuilder.Append("λ x ");
            stringBuilder.Append(pos.y);
            stringBuilder.Append("φ");
            positionLabel.text = stringBuilder.ToString();
        }
        else
        {
            positionLabel.text = "";
        }

        var cameraDirXZ = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up);
        float heading = Vector3.SignedAngle(Vector3.forward, cameraDirXZ, Vector3.up);
        heading += rotationOffset;
        heading %= 360f;
        
        directionLabel.style.rotate = new StyleRotate(new Rotate(new Angle(heading)));
    }
}
