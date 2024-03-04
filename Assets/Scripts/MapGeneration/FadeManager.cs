using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class FadeManager : MonoBehaviour, IObjectGenerator
{
    public CameraController cameraController;
    public Vector3 cameraStartPosition;
    public Quaternion cameraStartRotation;

    public int Priority { get { return -1; } }
    public bool NewGameOnly { get { return false; } }
    public bool RunModeOnly { get { return true; } }

    public void Clear()
    {
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        cameraController.transform.position = cameraStartPosition;
        cameraController.transform.rotation = cameraStartRotation;
        yield return null;
    }
}
