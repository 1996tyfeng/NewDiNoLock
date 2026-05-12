using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NewDiNoLock.Editor
{
    public static class BuildAutomation
    {
        private const string WindowsBuildRelativePath = "Builds/Windows/NewDiNoLock.exe";
        private const string AppIconPath = "Assets/NewDiNoLock/Art/Icons/AppIcon.png";

        [MenuItem("NewDiNoLock/Build/Windows")]
        public static void BuildWindowsFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.Log("Windows build canceled because modified scenes were not saved.");
                return;
            }

            var outputPath = BuildWindowsPlayer();
            EditorUtility.DisplayDialog("Build Complete", $"Windows build succeeded:\n{outputPath}", "OK");
        }

        public static void BuildWindows()
        {
            BuildWindowsPlayer();
        }

        private static string BuildWindowsPlayer()
        {
            ApplyAppIcon();

            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            if (string.IsNullOrEmpty(projectRoot))
            {
                throw new InvalidOperationException("Unable to resolve Unity project root.");
            }

            var repoRoot = Directory.GetParent(projectRoot)?.FullName;
            if (string.IsNullOrEmpty(repoRoot))
            {
                throw new InvalidOperationException("Unable to resolve repository root.");
            }

            var outputPath = Path.Combine(repoRoot, WindowsBuildRelativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                throw new InvalidOperationException("No enabled scenes found in EditorBuildSettings.");
            }

            var report = BuildPipeline.BuildPlayer(
                scenes,
                outputPath,
                BuildTarget.StandaloneWindows64,
                BuildOptions.None);

            var summary = report.summary;
            UnityEngine.Debug.Log(
                $"Build result: {summary.result}, output: {outputPath}, " +
                $"errors: {summary.totalErrors}, warnings: {summary.totalWarnings}, size: {summary.totalSize}");

            if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Build failed: {summary.result}");
            }

            return outputPath;
        }

        public static void ApplyAppIcon()
        {
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AppIconPath);
            if (icon == null)
            {
                Debug.LogWarning($"App icon was not found at {AppIconPath}.");
                return;
            }

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
            var standaloneIconSizes = PlayerSettings.GetIconSizes(NamedBuildTarget.Standalone, IconKind.Application);
            var standaloneIcons = standaloneIconSizes.Length > 0
                ? standaloneIconSizes.Select(_ => icon).ToArray()
                : new[] { icon };
            PlayerSettings.SetIcons(NamedBuildTarget.Standalone, standaloneIcons, IconKind.Application);
            AssetDatabase.SaveAssets();
        }
    }
}
