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

    public Vector3 cameraStartPosition;
    public Quaternion cameraStartRotation;

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

    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private bool movingToPoint;
    private float zoomHeight;
    private float zoomVectorMaxLength;

    public int Priority { get { return -50; } }

    public bool NewGameOnly { get { return false; } }

    public bool RunModeOnly { get { return true; } }

    void Awake()
    {
        cameraActions = new CameraControlActions();
        cameraTransform  = GetComponentInChildren<Camera>().transform;
    }

    void OnEnable()
    {
        zoomHeight = 0.5f;
        zoomVectorMaxLength = cameraTransform.localPosition.magnitude * 2;
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
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        transform.position = cameraStartPosition;
        transform.rotation = cameraStartRotation;
        yield return null;
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

        transform.position += targetDirection * speed * Time.fixedDeltaTime;
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
            zoomHeight = zoomHeight + zoomDelta * stepSize;
            zoomHeight = Mathf.Clamp(zoomHeight, minZoomFactor, 1f);
        }
    }

    private void UpdateCameraPosition()
    {
        var zoomTarget = (cameraTransform.localPosition).normalized * zoomHeight * zoomVectorMaxLength;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.fixedDeltaTime * zoomSpeed);
    }

    public void MoveTo(Vector3 target)
    {
        targetPosition = target;
        targetDirection = (target - transform.position).normalized;
        movingToPoint = true;
    }

    public CameraSaveData GetSaveData()
    {
        return new CameraSaveData
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

    public void LoadState(CameraSaveData cameraData)
    {
        cameraActions.Disable();

        transform.position = new Vector3(cameraData.positionX, cameraData.positionY, cameraData.positionZ);
        transform.rotation = Quaternion.Euler(cameraData.rotationX, cameraData.rotationY, cameraData.rotationZ);

        targetPosition = Vector3.zero;
        targetDirection = Vector3.zero;
        movingToPoint = false;
        
        cameraTransform.localPosition = new Vector3(cameraData.cameraPositionX, cameraData.cameraPositionY, cameraData.cameraPositionZ);
        cameraTransform.localRotation = Quaternion.Euler(cameraData.cameraRotationX, cameraData.cameraRotationY, cameraData.cameraRotationZ);
        zoomHeight = cameraData.zoomHeight;

        cameraActions.Enable();
    }
}