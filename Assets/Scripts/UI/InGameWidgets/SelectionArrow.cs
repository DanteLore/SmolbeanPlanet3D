using UnityEngine;

public class SelectionArrow : MonoBehaviour
{
    public float revolutionsPerSecond = 0.5f;
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = GameObject.Find("Main Camera").transform;
    }

    void Update()
    {
        var n = cameraTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(n.x, 0, n.z));
    }
}
