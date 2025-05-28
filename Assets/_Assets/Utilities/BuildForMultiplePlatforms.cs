#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UnityBuildForMultiplePlatforms : MonoBehaviour
{
    static List<SupportedPlatform> platforms = new List<SupportedPlatform> {
        new SupportedPlatform(BuildTarget.StandaloneOSX, "../Builds/TESTING/MacOS/Game.app"),
        new SupportedPlatform(BuildTarget.StandaloneWindows64, "../Builds/TESTING/Win64/Game.exe")
        // new SupportedPlatform(BuildTarget.StandaloneLinux64, "Linux64/Game")
    };

    [MenuItem("Tools/My Game/Build")]
    public static void BuildGame()
    {
        BuildForAllPlatforms();
    }

    static void BuildForAllPlatforms()
    {
        string[] allScenes = CollectScenes().ToArray();

        foreach (var platform in platforms)
        {
            Debug.Log($"Building {platform.executablePath}");
            BuildForPlatform(allScenes, platform.target, platform.executablePath);
        }
    }

    static void BuildForPlatform(string[] scenes, BuildTarget target, string platformPath, BuildOptions options = BuildOptions.None)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = platformPath;
        buildPlayerOptions.target = target;
        buildPlayerOptions.options = options;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    static List<string> CollectScenes()
    {
        var projectScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        var scenePaths = projectScenes.Select(s => s.path).ToList();
        return scenePaths;
    }
}

class SupportedPlatform
{
    public BuildTarget target;
    public string executablePath;

    public SupportedPlatform(BuildTarget _target, string _executablePath)
    {
        target = _target;
        executablePath = _executablePath;
    }
}

#endif