using System;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
            SaveScreenshot();
    }

    private void SaveScreenshot()
    {
        string dt = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string name = $"screenshot-{dt}.png";
        string dir = Path.Join(Application.persistentDataPath, "Screenshots");
        string filename = Path.Join(dir, name);
        Directory.CreateDirectory(dir);
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log($"Screenshot saved to {filename}");
    }
}
