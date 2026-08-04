using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only tools for turning the active Unity scene into Toolkit scene JSON.
// The data classes and tracked-object components live in System/Runtime so they
// can also be used later by a runtime viewer or publishing system.
public static class ToolkitSceneSerializer
{
    // Creates (or selects) the scene-level settings object. Its published name
    // and generated scene ID are saved as part of the Unity scene.
    [MenuItem("Game-Toolkit/Scene Publishing Settings")]
    public static void SelectScenePublishingSettings()
    {
        // Work with whichever scene is currently active in the editor.
        Scene scene = SceneManager.GetActiveScene();
        // Reuse existing settings or create them when missing.
        ToolkitSceneMetadata metadata = GetOrCreateSceneMetadata(scene);

        // Show the settings object in the Hierarchy and Inspector.
        Selection.activeGameObject = metadata.gameObject;
        EditorGUIUtility.PingObject(metadata.gameObject);
    }

    [MenuItem("Game-Toolkit/Developer/Test Export Scene JSON")]
    public static void TestExportSceneJson()
    {
        // Ask for the public-facing name before choosing where to save the JSON.
        ToolkitSceneNameWindow.ShowWindow();
    }

    // Serializes the active scene and presents Unity's standard save-file dialog.
    private static void ExportSceneJson()
    {
        // Convert the active scene into Toolkit's serializable data structure.
        Scene scene = SceneManager.GetActiveScene();
        ToolkitSceneData sceneData = Serialize(scene);
        // Pretty-print the JSON to make test exports easy to inspect.
        string json = JsonUtility.ToJson(sceneData, true);

        // Use the published title as the suggested filename.
        string defaultName = string.IsNullOrEmpty(sceneData.title) ? "toolkit-scene" : sceneData.title;
        // Let the user choose where this test export should be written.
        string outputPath = EditorUtility.SaveFilePanel(
            "Export Toolkit Scene JSON",
            "",
            defaultName + ".json",
            "json"
        );

        // Canceling the save dialog returns an empty path.
        if (string.IsNullOrEmpty(outputPath))
        {
            return;
        }

        // Write the finished JSON and reveal it for quick inspection.
        File.WriteAllText(outputPath, json);
        Debug.Log("Exported Toolkit scene JSON to: " + outputPath);
        EditorUtility.RevealInFinder(outputPath);
    }

    // Scene metadata is stored on a GameObject so Unity saves it inside the
    // .unity scene file. Creating it through Undo makes the action reversible.
    private static ToolkitSceneMetadata GetOrCreateSceneMetadata(Scene scene)
    {
        // Avoid creating duplicate settings objects.
        ToolkitSceneMetadata metadata = FindSceneMetadata(scene);
        if (metadata != null)
        {
            return metadata;
        }

        // Create a dedicated root object for Toolkit publishing settings.
        GameObject settingsObject = new GameObject("Toolkit Scene Settings");
        Undo.RegisterCreatedObjectUndo(settingsObject, "Create Toolkit Scene Settings");
        // Add persistent metadata and start with the Unity scene name.
        metadata = Undo.AddComponent<ToolkitSceneMetadata>(settingsObject);
        metadata.Initialize(scene.name);
        // Tell Unity that the new object needs to be saved with the scene.
        EditorSceneManager.MarkSceneDirty(scene);
        return metadata;
    }

    // Builds a plain data object that JsonUtility can convert to JSON.
    // This method does not write a file, which keeps serialization separate
    // from the editor UI used to choose a filename.
    public static ToolkitSceneData Serialize(Scene scene)
    {
        // Read the optional persistent publishing settings for this scene.
        ToolkitSceneMetadata metadata = FindSceneMetadata(scene);

        // Start with the Unity scene name as the default.
        string publishedName = scene.name;

        // Check whether this scene has Toolkit publishing settings.
        if (metadata != null)
        {
            // Check whether the user entered a meaningful published name.
            if (!string.IsNullOrWhiteSpace(metadata.publishedName))
            {
                // Use the custom name and remove extra spaces from its ends.
                publishedName = metadata.publishedName.Trim();
            }
        }

        // Create the top-level object that will become the JSON document.
        ToolkitSceneData sceneData = new ToolkitSceneData
        {
            sceneId = metadata == null ? scene.name : metadata.SceneId,
            title = publishedName
        };

        // Search each root and all of its children, including inactive objects.
        // This limits the search to the active scene rather than loaded assets
        // or objects belonging to another open scene.
        List<ToolkitSceneObject> trackedObjects = new List<ToolkitSceneObject>();
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            // Include tracked objects nested anywhere below this root.
            trackedObjects.AddRange(rootObject.GetComponentsInChildren<ToolkitSceneObject>(true));
        }

        foreach (ToolkitSceneObject trackedObject in trackedObjects)
        {
            // Phase one publishes only image objects that explicitly opt in.
            if (!trackedObject.includeInPublishedScene || trackedObject.objectType != "image")
            {
                continue;
            }

            Transform objectTransform = trackedObject.transform;

            // Export world-space transforms so the web viewer can reproduce the
            // placement without needing Unity's parent hierarchy.
            sceneData.objects.Add(new ToolkitObjectData
            {
                // The persistent ID links this Unity object to its published form.
                objectId = trackedObject.ObjectId,
                type = "image",
                // Prefer a public URL, falling back to the known source path/name.
                assetUrl = string.IsNullOrEmpty(trackedObject.assetUrl)
                    ? trackedObject.sourceAssetPath
                    : trackedObject.assetUrl,
                transform = new ToolkitTransformData
                {
                    // JsonUtility serializes Vector3 values as x/y/z fields.
                    position = objectTransform.position,
                    rotation = objectTransform.eulerAngles,
                    scale = objectTransform.lossyScale
                }
            });
        }

        return sceneData;
    }

    // There should normally be one metadata component per scene. Returning the
    // first match keeps this initial implementation small and predictable.
    private static ToolkitSceneMetadata FindSceneMetadata(Scene scene)
    {
        // Check every root because the metadata object may appear anywhere.
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            ToolkitSceneMetadata metadata = rootObject.GetComponentInChildren<ToolkitSceneMetadata>(true);
            if (metadata != null)
            {
                // Stop as soon as the scene's settings are found.
                return metadata;
            }
        }

        return null;
    }

    // Small modal-like utility window shown by the test export command. It
    // remembers an existing published name and stores any edited value back in
    // the scene before continuing to the file-save dialog.
    private class ToolkitSceneNameWindow : EditorWindow
    {
        private string publishedName;

        public static void ShowWindow()
        {
            // Read the current name so returning users can edit it.
            Scene scene = SceneManager.GetActiveScene();
            ToolkitSceneMetadata metadata = FindSceneMetadata(scene);

            // Create a small fixed-size utility window rather than a docked tab.
            ToolkitSceneNameWindow window = CreateInstance<ToolkitSceneNameWindow>();
            window.titleContent = new GUIContent("Name Toolkit Scene");
            window.publishedName = metadata != null && !string.IsNullOrWhiteSpace(metadata.publishedName)
                ? metadata.publishedName
                : scene.name;
            // Keep the prompt compact and consistent.
            window.minSize = new Vector2(360f, 145f);
            window.maxSize = window.minSize;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            // Draw the editable published-name field.
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Published Scene Name", EditorStyles.boldLabel);
            publishedName = EditorGUILayout.TextField(publishedName);

            // Explain that persistence requires saving the Unity scene.
            EditorGUILayout.Space(6f);
            EditorGUILayout.HelpBox(
                "This name is stored with the Unity scene. Save the scene to keep it.",
                MessageType.Info
            );

            // Prevent exporting with a blank or whitespace-only title.
            EditorGUILayout.Space(8f);
            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(publishedName)))
            {
                if (GUILayout.Button("Continue to Export"))
                {
                    SaveNameAndExport();
                }
            }
        }

        private void SaveNameAndExport()
        {
            // Ensure there is a persistent place to store the confirmed name.
            Scene scene = SceneManager.GetActiveScene();
            ToolkitSceneMetadata metadata = GetOrCreateSceneMetadata(scene);

            // Record the edit for Cmd/Ctrl+Z and mark the scene as modified so
            // Unity knows the persistent name needs to be saved.
            Undo.RecordObject(metadata, "Set Toolkit Scene Name");
            metadata.publishedName = publishedName.Trim();
            EditorUtility.SetDirty(metadata);
            EditorSceneManager.MarkSceneDirty(scene);

            // Close this prompt before opening the system save dialog.
            Close();
            ExportSceneJson();
        }
    }
}
