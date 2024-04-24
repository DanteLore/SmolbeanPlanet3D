using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// https://www.youtube.com/watch?v=pJQndtJ2rk0
// https://www.youtube.com/watch?v=ZSP3bFaZm-o
public class CameraController : MonoBehaviour
{    
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float speedMin = 30f;
    [SerializeField] private float speedMax = 300f;
    [SerializeField] private float rotateSpeedMin = 90f;
    [SerializeField] private float rotateSpeedMax = 180f;
    [SerializeField] private float zoomStep = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomDistanceMin = 10f;
    [SerializeField] private float zoomDistanceMax = 500f;
    [SerializeField] private Vector3 lookAngleMin = new(0f, 0.1f, 0.8f);
    [SerializeField] private Vector3 lookAngleMax = new(0f, 0.8f, -0.1f);

    private GridManager gridManager;
    private CinemachineTransposer transposer;
    private float zoomDistance;
    private float targetZoom;
    private float speed;
    private float rotateSpeed;
    private SmolbeanInputActions actions;
    private InputAction movementAction;
    private InputAction rotationAction;
    private float rotationInput;
    private float zoomInput;

    void Awake()
    {
        actions = new SmolbeanInputActions();
        movementAction = actions.GodView.Movement;
        rotationAction = actions.GodView.RotateCamera;
        actions.GodView.ZoomCamera.performed += CameraZoomInput;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoomDistance = transposer.m_FollowOffset.magnitude;
        targetZoom = zoomDistance;

        actions.GodView.Enable();
    }

    private void CameraRotationInput(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<Vector2>().x;
    }

    private void CameraZoomInput(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<Vector2>().y / 100f;
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPaused)
            return;

        if(movementAction.inProgress)
        {
            Vector2 input = movementAction.ReadValue<Vector2>();
            speed = input.magnitude < 1.0f ? speedMin : Mathf.Lerp(speed, speedMax, Time.unscaledDeltaTime);
            Vector3 move = transform.forward * input.y + transform.right * input.x;
            move *= speed * Time.unscaledDeltaTime;
            var newPos = transform.position + move;
            float y = gridManager.GetMaxGridHightAt(newPos.x, newPos.z);
            y = float.IsNaN(y) ? 1f : y;
            y = Mathf.Max(y, 1f);
            transform.position = new Vector3(newPos.x, y, newPos.z);
        }

        if(rotationAction.activeControl != null && 
            (!rotationAction.activeControl.path.Contains("Mouse") || Mouse.current.rightButton.isPressed))
        {
            rotationInput = rotationAction.ReadValue<Vector2>().x;
            rotateSpeed = Mathf.Abs(rotationInput) < 1 ? rotateSpeedMin : Mathf.Lerp(rotateSpeed, rotateSpeedMax, Time.unscaledDeltaTime);
            rotationInput *= rotateSpeed * Time.unscaledDeltaTime;
            transform.eulerAngles += new Vector3(0f, rotationInput, 0f);
        }

        if(zoomInput > 0f)
            targetZoom -= zoomStep;
        else if(zoomInput < 0f)
            targetZoom += zoomStep;

        targetZoom = Mathf.Clamp(targetZoom, zoomDistanceMin, zoomDistanceMax);

        float zoomNorm = Mathf.InverseLerp(zoomDistanceMin, zoomDistanceMax, zoomDistance);
        Vector3 lookVector = Quaternion.Euler(0f, transform.rotation.y, 0f) * Vector3.Lerp(lookAngleMin, lookAngleMax, zoomNorm).normalized;
        
        zoomDistance = Mathf.Lerp(zoomDistance, targetZoom, Time.unscaledDeltaTime * zoomSpeed);
        transposer.m_FollowOffset = lookVector * zoomDistance;

        // Reset inputs we've digested
        zoomInput = 0f;
    }
}