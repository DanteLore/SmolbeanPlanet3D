using Cinemachine;
using UnityEngine;

// https://www.youtube.com/watch?v=pJQndtJ2rk0
public class CameraController : MonoBehaviour
{    
    public CinemachineVirtualCamera virtualCamera;
    public float speedMin = 30f;
    public float speedMax = 300f;
    public float rotateSpeedMin = 90f;
    public float rotateSpeedMax = 180f;
    public float mouseRotationSpeedMax = 2f;
    public float zoomStep = 10f;
    public float zoomSpeed = 5f;
    public float zoomDistanceMin = 10f;
    public float zoomDistanceMax = 500f;
    public Vector3 lookAngleMin = new(0f, 0.1f, 0.8f);
    public Vector3 lookAngleMax = new(0f, 0.8f, -0.1f);

    private GridManager gridManager;
    private CinemachineTransposer transposer;
    private float zoomDistance;
    private float targetZoom;
    private float speed;
    private float rotateSpeed;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoomDistance = transposer.m_FollowOffset.magnitude;
        targetZoom = zoomDistance;
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPaused)
            return;

        Vector3 input = GetMovementInput();
        speed = input.magnitude < 1.0f ? speedMin : Mathf.Lerp(speed, speedMax, Time.unscaledDeltaTime);
        Vector3 move = transform.forward * input.z + transform.right * input.x;
        move *= speed * Time.unscaledDeltaTime;
        var newPos = transform.position + move;
        float y = gridManager.GetMaxGridHightAt(newPos.x, newPos.z);
        y = float.IsNaN(y) ? 1f : y;
        y = Mathf.Max(y, 1f);
        transform.position = new Vector3(newPos.x, y, newPos.z);

        float rotation = GetRotationInput();
        rotateSpeed = Mathf.Abs(rotation) < 1 ? rotateSpeedMin : Mathf.Lerp(rotateSpeed, rotateSpeedMax, Time.unscaledDeltaTime);
        rotation *= rotateSpeed * Time.unscaledDeltaTime;
        transform.eulerAngles += new Vector3(0f, rotation, 0f);

        float zoomInput = Input.mouseScrollDelta.y;

        if(zoomInput > 0f)
            targetZoom -= zoomStep;
        else if(zoomInput < 0f)
            targetZoom += zoomStep;

        targetZoom = Mathf.Clamp(targetZoom, zoomDistanceMin, zoomDistanceMax);

        float zoomNorm = Mathf.InverseLerp(zoomDistanceMin, zoomDistanceMax, zoomDistance);
        Vector3 lookVector = Quaternion.Euler(0f, transform.rotation.y, 0f) * Vector3.Lerp(lookAngleMin, lookAngleMax, zoomNorm).normalized;
        
        zoomDistance = Mathf.Lerp(zoomDistance, targetZoom, Time.unscaledDeltaTime * zoomSpeed);
        transposer.m_FollowOffset = lookVector * zoomDistance;
    }

    private static float GetRotationInput()
    {
        float rotation = 0f;
        if (Input.GetKey(KeyCode.Q))
            rotation -= 1f;
        if (Input.GetKey(KeyCode.E))
            rotation += 1f;
        return rotation;
    }

    private static Vector3 GetMovementInput()
    {
        Vector3 input = new(0f, 0f, 0f);

        if (Input.GetKey(KeyCode.W))
            input.z += 1.0f;
        if (Input.GetKey(KeyCode.A))
            input.x -= 1.0f;
        if (Input.GetKey(KeyCode.S))
            input.z -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            input.x += 1.0f;
        return input;
    }
}