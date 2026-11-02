using System;
using UnityEditor;
using UnityEngine;

// A server choice belongs to this local project, rather than to an individual scene.
public static class ToolkitServerSettings
{
    private static string PreferenceKey => "CommunityToolkit.DevelopmentMode." + Application.dataPath;
    public static event Action Changed;

    public static bool DevelopmentMode
    {
        get => EditorPrefs.GetBool(PreferenceKey, false);
        set
        {
            if (value == DevelopmentMode) return;
            EditorPrefs.SetBool(PreferenceKey, value);
            Changed?.Invoke();
        }
    }

    public static void DrawSettings()
    {
        EditorGUILayout.LabelField("Toolkit Server", EditorStyles.boldLabel);
        DevelopmentMode = EditorGUILayout.Toggle("Development Mode", DevelopmentMode);
        EditorGUILayout.HelpBox(DevelopmentMode
            ? "Uses the local development server at http://localhost:8000."
            : "Uses https://share.communitygametoolkit.org.", MessageType.Info);
    }
}

[CustomEditor(typeof(ToolkitSceneMetadata))]
public class ToolkitSceneMetadataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(12);
        ToolkitServerSettings.DrawSettings();
    }
}
