using System;
using System.IO;
using UnityEngine;

/// <summary>
/// F12 salva um PNG em persistentDataPath/shots — para screenshots da loja e do trailer.
/// Funciona também na build.
/// </summary>
public class ScreenshotCapture : MonoBehaviour
{
    [SerializeField] int superSize = 1;

    void Update()
    {
        if (!InputReader.WasPressed(GameKey.Screenshot)) return;

        string folder = Path.Combine(Application.persistentDataPath, "shots");
        Directory.CreateDirectory(folder);

        string file = Path.Combine(folder, "swarm_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
        ScreenCapture.CaptureScreenshot(file, superSize);

        Debug.Log("[SWARM] Screenshot salvo em: " + file);
    }
}
