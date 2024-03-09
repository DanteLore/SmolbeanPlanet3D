using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, IObjectGenerator
{
    private CameraControlActions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [Header("Default Position")]
    public Vector3 cameraStartPosition;
    public Quaternion cameraStartRotation;
    public float cameraStartTilt = 20;

    [Header("Horizontal Movement")]
    public float maxSpeed = 100f;
    public float acceleration = 20f;
    public float decelleraion = 40f;
    private float speed;

    [Header("Zoom")]
    public float stepSize = 0.01f;
    [Range(0f, 1f)]
    public float minZoomFactor = 0.05f;
    public float zoomSpeed = 8f;

    [Header("Rotation")]
    public float maxRotationSpeed = 2f;
    public float minTilt = 10.0f;
    public float maxTilt = 80.0f;

    public float flySpeedMetresPerSec = 20f;
    public float zoomVectorMaxLength = 80f;

    private float zoomHeight;

    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private bool movingToPoint;

    internal Vector3 gameStartPositon;

    public int Priority { get { return 500; } }
    public bool RunModeOnly { get { return true; } }

    void Awake()
    {
        cameraActions = new CameraControlActions();
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void OnEnable()
    {
        zoomHeight = 0.5f;
        cameraTransform.LookAt(transform);

        movement = cameraActions.Camera.Movement;
        cameraActions.Camera.RotateCamera.performed += RotateCamera;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraActions.Camera.Enable();

        movingToPoint = false;
        targetPosition = Vector3.zero;
        targetDirection = Vector3.zero;
    }

    void OnDisable()
    {
        cameraActions.Disable();
        cameraActions.Camera.RotateCamera.performed -= RotateCamera;
        cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
        cameraActions.Camera.Disable();
    }

    void Update()
    {
        if(!GameStateManager.Instance.IsPaused)
        {
            GetKeyboardMovement();
        }
    }

    void FixedUpdate()
    {
        if(!GameStateManager.Instance.IsPaused)
        {
            UpdateSpeed();
            UpdateBasePosition();
            UpdateCameraPosition();
        }
    }

    public void Clear()
    {
        transform.SetPositionAndRotation(cameraStartPosition, cameraStartRotation);
        cameraTransform.rotation = Quaternion.Euler(cameraStartTilt, 0f, 0f);
        zoomHeight = 1f;
        cameraTransform.localPosition = zoomHeight * zoomVectorMaxLength * cameraTransform.localPosition.normalized;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        yield return FlyTo(gameStartPositon, transform.rotation, cameraTransform.rotation, 0.2f);
    }

    private void GetKeyboardMovement()
    {
        if(movement.inProgress)
        {
            if(movingToPoint)
            {
                targetPosition = Vector3.zero;
                movingToPoint = false;
            }

            Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight() + movement.ReadValue<Vector2>().y * GetCameraForward();

            // Account for sudden changes in direction
            speed *= (Vector3.Dot(inputValue.normalized, targetDirection) + 1) / 2;

            targetDirection = inputValue.normalized;
        }
    }

    private Vector3 GetCameraRight()
    {
        var right = cameraTransform.right;
        right.y = 0.0f;
        return right;
    }

    private Vector3 GetCameraForward()
    {
        var forward = cameraTransform.forward;
        forward.y = 0.0f;
        return forward;
    }

    private void UpdateSpeed()
    {
        if (movement.inProgress || movingToPoint && MoreThanASecondToGetToDestination())
            speed = Mathf.Min(maxSpeed, speed + Time.fixedDeltaTime * acceleration);
        else
            speed = Mathf.Max(0f, speed - Time.fixedDeltaTime * decelleraion);
    }

    private bool MoreThanASecondToGetToDestination()
    {
        return (transform.position - targetPosition).magnitude / speed > 1f;
    }

    private void UpdateBasePosition()
    {
        if(movingToPoint && (transform.position - targetPosition).sqrMagnitude < 1f)
        {
            movingToPoint = false;
            targetPosition = Vector3.zero;
        }

        transform.position += speed * Time.fixedDeltaTime * targetDirection;
    }
    
    private void RotateCamera(InputAction.CallbackContext inputValue)
    {
        if(!Mouse.current.rightButton.isPressed || GameStateManager.Instance.IsPaused)
            return;

        float y = inputValue.ReadValue<Vector2>().x * maxRotationSpeed + transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0.0f, y , 0.0f);

        float x = inputValue.ReadValue<Vector2>().y * -1 * maxRotationSpeed + cameraTransform.rotation.eulerAngles.x;
        x = Mathf.Clamp(x, minTilt, maxTilt);

        cameraTransform.rotation = Quaternion.Euler(x, y , 0.0f);
    }

    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        if(GameStateManager.Instance.IsPaused)
            return;

        float zoomDelta = -inputValue.ReadValue<Vector2>().y / 100.0f;

        if(Mathf.Abs(zoomDelta) > 0.1f)
        {
            zoomHeight += zoomDelta * stepSize;
            zoomHeight = Mathf.Clamp(zoomHeight, minZoomFactor, 1f);
        }
    }

    private void UpdateCameraPosition()
    {
        var zoomTarget = zoomHeight * zoomVectorMaxLength * cameraTransform.localPosition.normalized;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.fixedDeltaTime * zoomSpeed);
    }

    public void SaveTo(SaveFileData saveData)
    {
        saveData.cameraData = new CameraSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationX = transform.rotation.eulerAngles.x,
            rotationY = transform.rotation.eulerAngles.y,
            rotationZ = transform.rotation.eulerAngles.z,
            cameraPositionX = cameraTransform.localPosition.x,
            cameraPositionY = cameraTransform.localPosition.y,
            cameraPositionZ = cameraTransform.localPosition.z,
            cameraRotationX = cameraTransform.localRotation.eulerAngles.x,
            cameraRotationY = cameraTransform.localRotation.eulerAngles.y,
            cameraRotationZ = cameraTransform.localRotation.eulerAngles.z,
            zoomHeight = zoomHeight
        };
    }

    public IEnumerator Load(SaveFileData data)
    {
        if (data.cameraData != null)
        {
            Vector3 pos = new (data.cameraData.positionX, data.cameraData.positionY, data.cameraData.positionZ);
            Quaternion rot = Quaternion.Euler(data.cameraData.rotationX, data.cameraData.rotationY, data.cameraData.rotationZ);
            Quaternion cameraRot = Quaternion.Euler(data.cameraData.cameraRotationX, data.cameraData.cameraRotationY, data.cameraData.cameraRotationZ);
            float zoom = data.cameraData.zoomHeight;

            yield return FlyTo(pos, rot, cameraRot, zoom);
        }

        yield return null;
    }

    private IEnumerator FlyTo(Vector3 targetPosition, Quaternion targetRotation, Quaternion targetCameraRotation, float targetZoomHeight)
    {
        cameraActions.Disable();
        movingToPoint = true;

        var startPosition = transform.position;
        var startRotation = transform.rotation;
        var startCameraRotation = cameraTransform.rotation;
        var startZoomHeight = zoomHeight;

        float distance = (startPosition - targetPosition).magnitude;
        float duration = 1000 * distance / flySpeedMetresPerSec; // Assume speed
        DateTime startTime = DateTime.Now;

        yield return null;

        while(transform.position != targetPosition)
        {
            float t = (float)((DateTime.Now - startTime).TotalMilliseconds / duration);

            Vector3 pos = Vector3.Lerp(startPosition, targetPosition, t);
            Quaternion rot = Quaternion.Lerp(startRotation, targetRotation, t);
            transform.SetPositionAndRotation(pos, rot);
            zoomHeight = Mathf.Lerp(startZoomHeight, targetZoomHeight, t);
            cameraTransform.localPosition = zoomHeight * zoomVectorMaxLength * cameraTransform.localPosition.normalized;
            cameraTransform.localRotation = Quaternion.Lerp(startCameraRotation, targetCameraRotation, t);

            yield return null;
        }

        this.targetPosition = targetPosition;

        yield return null;

        movingToPoint = false;
        cameraActions.Enable();
    }
}