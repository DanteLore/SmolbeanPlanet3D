using System;
using UnityEngine;
using UnityEngine.UIElements;

public class NavigationControls : MonoBehaviour
{
    public GameObject cameraPositionObject;
    public GridManager gridManager;
    public string groundLayer = "Ground";
    public string seaLayer = "Sea";
    public float rotationOffset = -48f;

    private Label positionLabel;
    private Label directionLabel;

    private int groundLayerMask;
    
    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        
        groundLayerMask = LayerMask.GetMask(groundLayer, seaLayer);
        positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        directionLabel = document.rootVisualElement.Q<Label>("directionLabel");
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

        var cameraDirXZ = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Vector3.Normalize(cameraDirXZ);
        float heading = Vector3.SignedAngle(Vector3.forward, cameraDirXZ, Vector3.up);
        heading += rotationOffset;
        heading %= 360f;
        
        directionLabel.style.rotate = new StyleRotate(new Rotate(new Angle(heading)));
    }
}
