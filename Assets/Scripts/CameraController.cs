using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControlActions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;

    [Header("Horizontal Movement")]
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float damping = 15f;
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

    [Header("Screen Edge Movement")]
    public float edgeTolerance = 0.05f;
    private bool useScreenEdge = true;

    private Vector3 targetPosition;
    private float zoomHeight;
    private float zoomVectorMaxLength;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;


    void Awake()
    {
        cameraActions = new CameraControlActions();
        cameraTransform  = GetComponentInChildren<Camera>().transform;
    }

    void OnEnable()
    {
        if(Application.isEditor)
            useScreenEdge = false; // Disabe bump scroll in editor

        zoomHeight = 0.5f;
        zoomVectorMaxLength = cameraTransform.localPosition.magnitude * 2;
        cameraTransform.LookAt(transform);

        lastPosition = transform.position;
        movement = cameraActions.Camera.Movement;
        cameraActions.Camera.RotateCamera.performed += RotateCamera;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraActions.Camera.Enable();
    }

    void OnDisable()
    {
        cameraActions.Disable();
        cameraActions.Camera.RotateCamera.performed -= RotateCamera;
        cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
    }

    void Update()
    {
        if(!GameStateManager.Instance.IsPaused)
        {
            GetKeyboardMovement();
            CheckMouseAtScreenEdge();
            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
        }
    }

    private void UpdateVelocity()
    {
        horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0.0f;
        lastPosition = transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight() + movement.ReadValue<Vector2>().y * GetCameraForward();
        inputValue = inputValue.normalized;

        if(inputValue.sqrMagnitude > 0.1f)
        {
            targetPosition += inputValue;
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

    private void UpdateBasePosition()
    {
        if(targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }
    
    private void RotateCamera(InputAction.CallbackContext inputValue)
    {
        if(!Mouse.current.rightButton.isPressed)
            return;

        float y = inputValue.ReadValue<Vector2>().x * maxRotationSpeed + transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0.0f, y , 0.0f);

        float x = inputValue.ReadValue<Vector2>().y * -1 * maxRotationSpeed + cameraTransform.rotation.eulerAngles.x;
        x = Mathf.Clamp(x, minTilt, maxTilt);
        cameraTransform.rotation = Quaternion.Euler(x, y , 0.0f);
    }

    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y / 100.0f;

        if(Mathf.Abs(value) > 0.1f)
        {
            zoomHeight = zoomHeight + value * stepSize;
            zoomHeight = Mathf.Clamp(zoomHeight, minZoomFactor, 1f);
        }
    }

    private void UpdateCameraPosition()
    {
        var zoomTarget = (cameraTransform.localPosition).normalized * zoomHeight * zoomVectorMaxLength;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomSpeed);
    }

    private void CheckMouseAtScreenEdge()
    {
        if(!useScreenEdge)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if(mousePosition.x < edgeTolerance * Screen.width)
        {
            moveDirection += -GetCameraRight();
        }
        else if(mousePosition.x > (1.0f - edgeTolerance) * Screen.width)
        {
            moveDirection += GetCameraRight();
        }

        if(mousePosition.y < edgeTolerance * Screen.height)
        {
            moveDirection += -GetCameraForward();
        }
        else if(mousePosition.y > (1.0f - edgeTolerance) * Screen.height)
        {
            moveDirection += GetCameraForward();
        }

        targetPosition += moveDirection;
    }
}
