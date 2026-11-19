#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Gera a build Windows. Menu: SWARM > Build Windows.
/// Batch: Unity.exe -batchmode -quit -executeMethod BuildScript.BuildWindowsBatch
/// </summary>
public static class BuildScript
{
    const string ScenePath = "Assets/Swarm/Scenes/Swarm.unity";
    const string OutputPath = "Builds/Windows/SWARM.exe";

    [MenuItem("SWARM/Build Windows", priority = 10)]
    public static void BuildWindowsMenu()
    {
        bool ok = Build();
        EditorUtility.DisplayDialog("SWARM",
            ok ? "Build gerada em:\n" + OutputPath : "Build falhou. Veja o Console.",
            "OK");
    }

    public static void BuildWindowsBatch()
    {
        bool ok = Build();
        if (!ok) EditorApplication.Exit(1);
    }

    static bool Build()
    {
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
        {
            Debug.LogError("[SWARM] Windows Build Support não está instalado no editor.");
            return false;
        }

        string directory = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        PlayerSettings.companyName = "Scoty";
        PlayerSettings.productName = "SWARM";
        PlayerSettings.bundleVersion = "0.7.0";
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.runInBackground = true;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

        var options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            float megabytes = summary.totalSize / (1024f * 1024f);
            Debug.Log("[SWARM] Build OK: " + OutputPath +
                      " (" + megabytes.ToString("0.0") + " MB) em " + summary.totalTime);
            return true;
        }

        Debug.LogError("[SWARM] Build falhou: " + summary.result + " (" + summary.totalErrors + " erros)");
        return false;
    }
}
#endif
