using UnityEngine;

public class BuffController : MonoBehaviour
{
    public static BuffController Instance { get; private set; }

    public BuffSpec[] BuffSpecs;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
