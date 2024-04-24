using UnityEngine;

public class FollowCameraTarget : MonoBehaviour
{
    private void OnDestroy()
    {
        if(FollowCameraController.Instance.Target == this)
            FollowCameraController.Instance.SetTarget(null);
    }
}
