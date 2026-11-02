using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolkitNewSceneWindow : EditorWindow
{
    private const string ScenesFolder = "Assets/Scenes";
    private const string GroundedPlayerPrefabPath = "Assets/Community-Game-Development-Toolkit/CGDT Components/Player/Player-Grounded.prefab";
    private const string FlyingPlayerPrefabPath = "Assets/Community-Game-Development-Toolkit/CGDT Components/Player/Player-Flying.prefab";
    [SerializeField] private bool flying;
    [SerializeField] private string sceneName = "New Scene";
    private string error;
    private bool focusName = true;

    [MenuItem("Game-Toolkit/Create New Scene (with ground)")]
    private static void ShowGroundedDialog() => ShowDialog(false);

    [MenuItem("Game-Toolkit/Create New Scene (flying)")]
    private static void ShowFlyingDialog() => ShowDialog(true);

    private static void ShowDialog(bool flying)
    {
        var window = CreateInstance<ToolkitNewSceneWindow>();
        window.flying = flying;
        window.titleContent = new GUIContent(flying ? "Create New Scene (flying)" : "Create New Scene (with ground)");
        window.minSize = new Vector2(400, 170);
        window.maxSize = new Vector2(600, 220);
        window.ShowModalUtility();
    }

    [MenuItem("Game-Toolkit/Create New Scene (with ground)", true)]
    [MenuItem("Game-Toolkit/Create New Scene (flying)", true)]
    private static bool CanCreate() => !EditorApplication.isCompiling &&
        !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode;

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUI.SetNextControlName("SceneName");
        sceneName = EditorGUILayout.TextField("Scene name", sceneName);
        if (focusName)
        {
            EditorGUI.FocusTextInControl("SceneName");
            focusName = false;
        }
        EditorGUILayout.LabelField("Save in", ScenesFolder);
        if (!string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox(error, MessageType.Error);
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Cancel", GUILayout.Width(90))) { Close(); GUIUtility.ExitGUI(); }
        bool create = GUILayout.Button("Create", GUILayout.Width(90));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Escape) { Close(); GUIUtility.ExitGUI(); }
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
            {
                Event.current.Use();
                create = true;
            }
        }
        if (create) CreateScene();
    }

    private static string ValidateName(string value)
    {
        string name = value.Trim();
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Enter a scene name.");
        if (name.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - 6).Trim();
        if (string.IsNullOrEmpty(name) || name == "." || name == ".." || name.EndsWith(".", StringComparison.Ordinal))
            throw new ArgumentException("Enter a valid scene name without a trailing period.");
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.IndexOfAny("<>:\"/\\|?*".ToCharArray()) >= 0)
            throw new ArgumentException("Scene names cannot contain < > : \" / \\ | ? *.");
        foreach (char character in name)
            if (char.IsControl(character)) throw new ArgumentException("Scene names cannot contain control characters.");
        return name;
    }

    private static void PopulateScene(Scene scene, GameObject playerPrefab, bool flying)
    {
        // Keep the standard directional light, replacing the standalone camera with the player's.
        foreach (GameObject root in scene.GetRootGameObjects())
            if (root.GetComponent<Camera>() != null) UnityEngine.Object.DestroyImmediate(root);

        SceneManager.SetActiveScene(scene);
        if (!flying)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            SceneManager.MoveGameObjectToScene(floor, scene);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.rotation = Quaternion.identity;
            // Unity's plane is 10 x 10 units, giving a 40 x 40 unit walkable floor.
            floor.transform.localScale = new Vector3(4f, 1f, 4f);
        }

        var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
        if (player == null) throw new IOException("Could not create the player.");
        player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (flying)
        {
            player.transform.position = new Vector3(0f, 1f, 0f);
        }
        else
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller == null) throw new IOException("The grounded player needs a CharacterController.");
            // Place the bottom of the controller just above y=0, preserving the prefab's scale.
            float bottom = player.transform.TransformPoint(controller.center).y -
                controller.height * Mathf.Abs(player.transform.lossyScale.y) * 0.5f;
            player.transform.position = new Vector3(0f, 0.05f - bottom, 0f);
        }

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null) throw new IOException("The player needs a camera.");
        playerCamera.tag = "MainCamera";
        PrefabUtility.RecordPrefabInstancePropertyModifications(player.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(playerCamera.gameObject);
    }

    private void CreateScene()
    {
        Scene created = default;
        Scene previous = SceneManager.GetActiveScene();
        bool opened = false;
        try
        {
            string name = ValidateName(sceneName);
            string path = ScenesFolder + "/" + name + ".unity";
            if (File.Exists(path) || File.Exists(path + ".meta") || Directory.Exists(path))
                throw new IOException("A scene or asset with that name already exists. Choose another name.");
            string playerPrefabPath = flying ? FlyingPlayerPrefabPath : GroundedPlayerPrefabPath;
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);
            if (playerPrefab == null) throw new IOException("Player prefab not found: " + playerPrefabPath);
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!AssetDatabase.IsValidFolder(ScenesFolder))
            {
                if (string.IsNullOrEmpty(AssetDatabase.CreateFolder("Assets", "Scenes")))
                    throw new IOException("Could not create Assets/Scenes.");
            }
            // Save successfully before replacing the scenes currently open in the editor.
            created = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            PopulateScene(created, playerPrefab, flying);
            if (!EditorSceneManager.SaveScene(created, path)) throw new IOException("The scene could not be saved.");
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            opened = true;
            Debug.Log("Created scene: " + path);
        }
        catch (Exception exception)
        {
            error = exception.Message;
            Debug.LogException(exception);
        }
        finally
        {
            if (!opened && created.IsValid() && created.isLoaded)
            {
                EditorSceneManager.CloseScene(created, true);
                if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
            }
        }
        if (opened) { Close(); GUIUtility.ExitGUI(); }
    }
}
