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
        ToolkitSceneNameWindow.ShowWindow(false);
    }

    [MenuItem("Game-Toolkit/Developer/Test Export Scene Bundle")]
    public static void TestExportSceneBundle()
    {
        // Use the same persistent-name prompt before exporting the complete bundle.
        ToolkitSceneNameWindow.ShowWindow(true);
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

    // Writes scene.json and all referenced images into one portable folder.
    private static void ExportSceneBundle()
    {
        if (!TryBuildSceneBundle(out ToolkitSceneBundle bundle))
        {
            return;
        }

        string defaultFolderName = MakeSafeFileName(bundle.sceneData.title);
        string outputFolder = EditorUtility.SaveFolderPanel(
            "Export Toolkit Scene Bundle",
            "",
            defaultFolderName
        );

        if (string.IsNullOrEmpty(outputFolder))
        {
            return;
        }

        string imagesFolder = Path.Combine(outputFolder, "images");
        Directory.CreateDirectory(imagesFolder);

        foreach (KeyValuePair<string, byte[]> encodedImage in bundle.images)
        {
            string imagePath = Path.Combine(imagesFolder, encodedImage.Key);
            File.WriteAllBytes(imagePath, encodedImage.Value);
        }

        string jsonPath = Path.Combine(outputFolder, "scene.json");
        File.WriteAllText(jsonPath, bundle.json);

        Debug.Log("Exported Toolkit scene bundle to: " + outputFolder);
        EditorUtility.RevealInFinder(outputFolder);
    }

    // Build one validated bundle in memory so local export and server publishing
    // always send the exact same JSON and image files.
    public static bool TryBuildSceneBundle(out ToolkitSceneBundle bundle)
    {
        bundle = null;

        Scene scene = SceneManager.GetActiveScene();
        GetOrCreateSceneMetadata(scene);

        ToolkitSceneData sceneData = Serialize(scene);
        List<ToolkitSceneObject> trackedImages = FindTrackedImages(scene);
        Dictionary<string, byte[]> encodedImages = new Dictionary<string, byte[]>();

        foreach (ToolkitSceneObject trackedImage in trackedImages)
        {
            string objectId = trackedImage.ObjectId;
            string imageFileName = objectId + ".png";

            if (encodedImages.ContainsKey(imageFileName))
            {
                Debug.LogError("Duplicate Toolkit object ID prevents bundle export: " + objectId);
                EditorUtility.DisplayDialog(
                    "Scene Bundle Build Failed",
                    "Two image objects have the same persistent ID: " + objectId,
                    "OK"
                );
                return false;
            }

            SpriteRenderer renderer = trackedImage.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null || renderer.sprite.texture == null)
            {
                Debug.LogError("Tracked image has no readable SpriteRenderer: " + trackedImage.name);
                EditorUtility.DisplayDialog(
                    "Scene Bundle Build Failed",
                    "The tracked image '" + trackedImage.name + "' has no sprite texture.",
                    "OK"
                );
                return false;
            }

            try
            {
                encodedImages.Add(imageFileName, renderer.sprite.texture.EncodeToPNG());
            }
            catch (UnityException exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Scene Bundle Build Failed",
                    "The texture on '" + trackedImage.name + "' is not readable and could not be exported.",
                    "OK"
                );
                return false;
            }

            ToolkitObjectData objectData = sceneData.objects.Find(
                item => item.objectId == objectId
            );

            if (objectData != null)
            {
                objectData.assetUrl = "images/" + imageFileName;
            }
        }

        bundle = new ToolkitSceneBundle
        {
            sceneData = sceneData,
            json = JsonUtility.ToJson(sceneData, true),
            images = encodedImages
        };

        return true;
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

        // Find the scene's player by looking for its KeyboardMove component.
        KeyboardMove player = FindPlayer(scene);

        if (player != null)
        {
            // The camera's pitch is controlled separately from the player root.
            MouseLookVertical verticalLook =
                player.GetComponentInChildren<MouseLookVertical>(true);
            float verticalLookAngle = verticalLook == null
                ? 0f
                : Mathf.DeltaAngle(0f, verticalLook.transform.localEulerAngles.x);

            // Copy the public movement settings and starting transform into JSON data.
            sceneData.player = new ToolkitPlayerData
            {
                flying = player.flying,
                speed = player.speed,
                gravity = player.gravity,
                jumpSpeed = player.jumpSpeed,
                minFall = player.minFall,
                pushForce = player.pushForce,
                position = player.transform.position,
                rotation = player.transform.eulerAngles,
                verticalLookAngle = verticalLookAngle
            };

            if (verticalLook == null)
            {
                Debug.LogWarning(
                    "The player has no MouseLookVertical component; exporting a neutral look angle."
                );
            }
        }
        else
        {
            Debug.LogWarning("No player with a KeyboardMove component was found in the active scene.");
        }

        // Search each root and all of its children, including inactive objects.
        // This limits the search to the active scene rather than loaded assets
        // or objects belonging to another open scene.
        List<ToolkitSceneObject> trackedObjects = FindTrackedImages(scene);

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

    // Collect all opted-in image objects from this scene, including inactive ones.
    private static List<ToolkitSceneObject> FindTrackedImages(Scene scene)
    {
        List<ToolkitSceneObject> trackedImages = new List<ToolkitSceneObject>();

        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            // Include tracked objects nested anywhere below this root.
            ToolkitSceneObject[] trackedObjects =
                rootObject.GetComponentsInChildren<ToolkitSceneObject>(true);

            foreach (ToolkitSceneObject trackedObject in trackedObjects)
            {
                if (trackedObject.includeInPublishedScene && trackedObject.objectType == "image")
                {
                    trackedImages.Add(trackedObject);
                }
            }
        }

        return trackedImages;
    }

    // Remove characters that cannot safely appear in a folder name.
    private static string MakeSafeFileName(string value)
    {
        string safeName = string.IsNullOrWhiteSpace(value) ? "toolkit-scene" : value.Trim();

        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(invalidCharacter, '-');
        }

        return safeName;
    }

    // Find the first player in the active scene, including inactive objects.
    private static KeyboardMove FindPlayer(Scene scene)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            KeyboardMove player = rootObject.GetComponentInChildren<KeyboardMove>(true);

            if (player != null)
            {
                return player;
            }
        }

        return null;
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
        private bool exportBundle;

        public static void ShowWindow(bool exportBundle)
        {
            // Read the current name so returning users can edit it.
            Scene scene = SceneManager.GetActiveScene();
            ToolkitSceneMetadata metadata = FindSceneMetadata(scene);

            // Create a small fixed-size utility window rather than a docked tab.
            ToolkitSceneNameWindow window = CreateInstance<ToolkitSceneNameWindow>();
            window.titleContent = new GUIContent("Name Toolkit Scene");
            window.exportBundle = exportBundle;
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
                string buttonLabel = exportBundle ? "Continue to Export Bundle" : "Continue to Export";
                if (GUILayout.Button(buttonLabel))
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

            if (exportBundle)
            {
                ExportSceneBundle();
            }
            else
            {
                ExportSceneJson();
            }
        }
    }
}
