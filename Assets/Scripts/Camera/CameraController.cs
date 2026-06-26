using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// https://www.youtube.com/watch?v=pJQndtJ2rk0
// https://www.youtube.com/watch?v=ZSP3bFaZm-o
public class CameraController : MonoBehaviour, IObjectGenerator
{
    // Inspector fields
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float speedMin = 1f;
    [SerializeField] private float speedMax = 10f;
    [SerializeField] private float speedAltitudeMultiplier = 30f;
    [SerializeField] private float rotateSpeedMin = 1f;
    [SerializeField] private float rotateSpeedMax = 10f;
    [SerializeField] private float mouseRotateSensitivity = 0.1f;
    [SerializeField] private float zoomStep = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomDistanceMin = 10f;
    [SerializeField] private float zoomDistanceMax = 500f;
    [SerializeField] private Vector3 lookAngleMin = new(0f, 0.1f, 0.8f);
    [SerializeField] private Vector3 lookAngleMax = new(0f, 0.8f, -0.1f);
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float startZoomDistance;

    // Private state
    private GridManager gridManager;
    private CinemachineTransposer transposer;
    private float zoomDistance;
    private float targetZoom;
    private float speed;
    private float rotateSpeed;
    private SmolbeanInputActions actions;
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction zoomAction;
    private float rotationInput;
    private float zoomInput;
    private bool isMouseOverUI;

    // IObjectGenerator
    public int Priority => 600;
    public bool RunModeOnly => true;

    // Unity lifecycle
    void Awake()
    {
        actions = new SmolbeanInputActions();
        movementAction = actions.GodView.Movement;
        rotationAction = actions.GodView.RotateCamera;
        zoomAction = actions.GodView.ZoomCamera;

        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);
    }

    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoomDistance = transposer.m_FollowOffset.magnitude;
        targetZoom = zoomDistance;

        GameStateManager.Instance.GamePauseStateChanged += PauseStateChanged;
        virtualCamera.gameObject.SetActive(!GameStateManager.Instance.IsPaused);

        actions.GodView.Enable();

        Clear();
    }

    void Update()
    {
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (GameStateManager.Instance.IsPaused)
            return;

        var trans = transform;

        // Zoom — only if mouse is not over UI
        if (!isMouseOverUI && !GameStateManager.Instance.IsPaused)
            zoomInput = zoomAction.ReadValue<Vector2>().y / 200f;

        float zoomNorm = Mathf.InverseLerp(zoomDistanceMin, zoomDistanceMax, zoomDistance);

        if (zoomInput > 0f)
            targetZoom -= zoomStep * PrefsManager.Instance.ZoomSpeed;
        else if (zoomInput < 0f)
            targetZoom += zoomStep * PrefsManager.Instance.ZoomSpeed;

        targetZoom = Mathf.Clamp(targetZoom, zoomDistanceMin, zoomDistanceMax);

        Vector3 lookVector = Quaternion.Euler(0f, trans.rotation.y, 0f) * Vector3.Lerp(lookAngleMin, lookAngleMax, zoomNorm).normalized;

        zoomDistance = Mathf.Lerp(zoomDistance, targetZoom, Time.unscaledDeltaTime * zoomSpeed);
        transposer.m_FollowOffset = lookVector * zoomDistance;

        float altMultiplier = Mathf.Lerp(1, speedAltitudeMultiplier * PrefsManager.Instance.AltitudeSpeedMultiplier, zoomNorm);

        // Move
        if (movementAction.inProgress)
        {
            Vector2 input = movementAction.ReadValue<Vector2>();
            speed = Mathf.Lerp(speed, speedMax * altMultiplier * PrefsManager.Instance.PanSpeed, Time.unscaledDeltaTime);
            Vector3 move = trans.forward * input.y + trans.right * input.x;
            move *= speed * Time.unscaledDeltaTime;
            var newPos = trans.position + move;
            float y = gridManager.GetMaxGridHightAt(newPos.x, newPos.z);
            y = float.IsNaN(y) ? 1f : y;
            y = Mathf.Max(y, 1f);
            trans.position = new Vector3(newPos.x, y, newPos.z);
        }
        else
        {
            speed = speedMin * altMultiplier;
        }

        // Rotate
        if (rotationAction.activeControl != null &&
            (!rotationAction.activeControl.path.Contains("Mouse") || Mouse.current.rightButton.isPressed))
        {
            rotationInput = rotationAction.ReadValue<Vector2>().x;

            bool isMouse = rotationAction.activeControl.path.Contains("Mouse");
            if (isMouse)
            {
                rotationInput *= mouseRotateSensitivity * PrefsManager.Instance.RotateSpeed;
            }
            else
            {
                rotateSpeed = Mathf.Abs(rotationInput) < 1
                    ? rotateSpeedMin
                    : Mathf.Lerp(rotateSpeed, rotateSpeedMax * PrefsManager.Instance.RotateSpeed, Time.unscaledDeltaTime);
                rotationInput *= rotateSpeed * Time.unscaledDeltaTime;
            }

            trans.eulerAngles += new Vector3(0f, rotationInput, 0f);
        }

        // Reset inputs we've digested
        zoomInput = 0f;
    }

    private void PauseStateChanged(object sender, bool isPaused)
    {
        virtualCamera.gameObject.SetActive(!isPaused);
    }

    // Public interface
    public void Clear()
    {
        targetZoom = startZoomDistance;
        SetTarget(startPosition);
    }

    public void SetTarget(Vector3 pos)
    {
        transform.position = pos;
    }

    // IObjectGenerator
    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        var pos = transform.position;
        var rot = transform.rotation.eulerAngles;
        var camPos = virtualCamera.transform.position;
        var camRot = virtualCamera.transform.rotation.eulerAngles;

        saveData.cameraData = new CameraSaveData {
            positionX = pos.x,
            positionY = pos.y,
            positionZ = pos.z,
            rotationX = rot.x,
            rotationY = rot.y,
            rotationZ = rot.z,
            cameraPositionX = camPos.x,
            cameraPositionY = camPos.y,
            cameraPositionZ = camPos.z,
            cameraRotationX = camRot.x,
            cameraRotationY = camRot.y,
            cameraRotationZ = camRot.z,
            zoomHeight = zoomDistance
        };
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        var d = data.cameraData;
        var pos = new Vector3(d.positionX, d.positionY, d.positionZ);
        var rot = new Vector3(d.rotationX, d.rotationY, d.rotationZ);
        var camPos = new Vector3(d.cameraPositionX, d.cameraPositionY, d.cameraPositionZ);
        var camRot = new Vector3(d.cameraRotationX, d.cameraRotationY, d.cameraRotationZ);

        transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
        virtualCamera.transform.SetPositionAndRotation(camPos, Quaternion.Euler(camRot));

        zoomDistance = d.zoomHeight;
        targetZoom = zoomDistance;

        return null;
    }
}
