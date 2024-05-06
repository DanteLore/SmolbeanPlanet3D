using Cinemachine;
using UnityEngine;

public class FollowCameraController : MonoBehaviour
{
    public static FollowCameraController Instance;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineComposer composer;
    private CinemachineTransposer transposer;

    public FollowCameraTarget Target { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    public void SetTarget(FollowCameraTarget target)
    {
        Target = target;

        if (Target == null && virtualCamera != null)
        {
            virtualCamera.gameObject.SetActive(false);
        }
        else
        {
            SetupCamera();
            virtualCamera.gameObject.SetActive(true);
        }
    }

    private void SetupCamera()
    {
        if(Target != null)
        {
            var targetTransform = Target.transform;

            float y = targetTransform.GetRendererBounds().max.y - targetTransform.transform.position.y;
            composer.m_TrackedObjectOffset.y = y;
            transposer.m_FollowOffset = new Vector3(0f, y * 2f, y * -4f);

            virtualCamera.Follow = targetTransform;
            virtualCamera.LookAt = targetTransform;
        }
    }
}
