using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ToolkitOpenSceneWindow : EditorWindow
{
    private const string ScenesFolder = "Assets/Scenes";
    private const string ExamplesFolder = "Assets/Community-Game-Development-Toolkit/Example Scenes";
    private string[] projectScenes = Array.Empty<string>();
    private string[] exampleScenes = Array.Empty<string>();
    private Vector2 scroll;
    private string error;

    [MenuItem("Game-Toolkit/Open Scene")]
    private static void ShowWindow()
    {
        var window = GetWindow<ToolkitOpenSceneWindow>(true, "Open Scene", true);
        window.minSize = new Vector2(400, 300);
        window.RefreshScenes();
        window.Show();
    }

    [MenuItem("Game-Toolkit/Open Scene", true)]
    private static bool CanOpen() => !EditorApplication.isCompiling &&
        !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode;

    private void OnEnable() => RefreshScenes();
    private void OnFocus() => RefreshScenes();
    private void OnProjectChange() => RefreshScenes();

    private static string[] FindScenes(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder)) return Array.Empty<string>();
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { folder });
        var paths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++) paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        Array.Sort(paths, StringComparer.OrdinalIgnoreCase);
        return paths;
    }

    private void RefreshScenes()
    {
        projectScenes = FindScenes(ScenesFolder);
        exampleScenes = FindScenes(ExamplesFolder);
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);
        if (!string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox(error, MessageType.Error);
        using (new EditorGUI.DisabledScope(!CanOpen()))
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawScenes("Your Scenes", ScenesFolder, projectScenes);
            EditorGUILayout.Space(15);
            DrawScenes("Toolkit Example Scenes", ExamplesFolder, exampleScenes);
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawScenes(string heading, string folder, string[] scenes)
    {
        EditorGUILayout.LabelField(heading, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(folder, EditorStyles.miniLabel);
        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in this folder.", MessageType.Info);
            return;
        }
        foreach (string path in scenes)
        {
            // Include subfolder names to distinguish scenes with the same filename.
            string label = path.Substring(folder.Length + 1);
            label = label.Substring(0, label.Length - Path.GetExtension(label).Length);
            if (GUILayout.Button(new GUIContent(label, path), GUILayout.Height(28)))
                OpenScene(path);
        }
    }

    private void OpenScene(string path)
    {
        error = null;
        try
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) == null)
                throw new IOException("That scene is no longer available. Please choose another scene.");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
        catch (Exception exception)
        {
            error = exception.Message;
            Debug.LogException(exception);
            RefreshScenes();
            return;
        }
        Close();
        GUIUtility.ExitGUI();
    }
}
