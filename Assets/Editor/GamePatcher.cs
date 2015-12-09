using System.IO;
using UnityEditor;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Diagnostics;

[InitializeOnLoad]
class GamePatcher
{
    [MenuItem("Game/Build All", priority = 101)]
    public static void BuildAll()
    {
        Build();
    }

    [MenuItem("Game/Build and Run All", priority = 102)]
    public static void BuildAndRunAll()
    {
        Build();
        Run("All");
    }

    [MenuItem("Game/Build and Debug Host", priority = 201)]
    public static void BuildAndDebugHost()
    {
        Build();
        Run("Client");
        PlayerPrefs.SetInt("GameHost", 1);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Game/Build and Debug Client", priority = 202)]
    public static void BuildAndDebugClient()
    {
        Build();
        Run("Host");
        PlayerPrefs.SetInt("GameClient", 1);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Game/Build and Debug Rift", priority = 203)]
    public static void BuildAndDebugRift()
    {
        Build();

        // start host using default parameters
        Run("Host", "-usedefaults");
        PlayerPrefs.SetInt("GameRift", 1);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Game/Debug Host", priority = 301)]
    public static void DebugHost()
    {
        if (EditorApplication.isPlaying)
            return;
        
        Run("Client");
        PlayerPrefs.SetInt("GameHost", 1);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Game/Debug Client", priority = 302)]
    public static void DebugClient()
    {
        if (EditorApplication.isPlaying)
            return;

        Run("Host");
        PlayerPrefs.SetInt("GameClient", 1);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Game/Run Host", priority = 401)]
    public static void RunHost()
    {
        Run("Host");
    }

    [MenuItem("Game/Run Client", priority = 402)]
    public static void RunClient()
    {
        Run("Client");
    }

    [MenuItem("Game/Run All", priority = 403)]
    public static void RunAll()
    {
        Run("All");
    }

    private static void Build()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        var loc = EditorUserBuildSettings.GetBuildLocation(target);
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

        BuildPipeline.BuildPlayer(scenes, loc, target, BuildOptions.None);
    }

    private static void Run(string patcher, string args = "")
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        var loc = EditorUserBuildSettings.GetBuildLocation(target);

        string targetPath = loc.Replace(".exe", "_" + patcher + ".exe");
        var targetFolder = Path.GetDirectoryName(targetPath);

        var pi = new ProcessStartInfo {
            FileName = targetPath,
            Arguments = args,
            WorkingDirectory = targetFolder,
        };

        Process.Start(pi);
    }
}
