using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ToolkitServerWindow : EditorWindow
{
    private const string LocalServerUrl = "http://localhost:8000";
    private const string HostedServerUrl = "https://share.communitygametoolkit.org";

    [SerializeField]
    private bool developmentMode = true;

    private ToolkitServerClient client;

    private Session currentSession;
    private Texture2D qrCode;
    private List<Texture2D> media = new();
    private string errorMessage;
    private bool isPublishing;
    private string publishedSceneUrl;
    
    //****** new sprite Placement

    private float minPlacementFraction = 0.5f;
    private float maxPlacementFraction = 2.0f;
    private float depthMultiplier = 2.0f;

    // Rotation
    private float maxXRotation = 60f;
    private float maxYRotation = 60f;
    private float maxZRotation = 90f;
    //******


    private string result = "";

    private string ServerUrl => developmentMode ? LocalServerUrl : HostedServerUrl;

    [MenuItem("Game-Toolkit/Developer/Toolkit Server")]
    public static void ShowWindow()
    {
        GetWindow<ToolkitServerWindow>("Toolkit Server");
    }

    private void OnEnable()
    {
        client = new ToolkitServerClient(ServerUrl);
    }

    private void OnGUI()
    {
        GUILayout.Label("Toolkit Server", EditorStyles.boldLabel);

        GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        developmentMode = EditorGUILayout.Toggle("Development Mode", developmentMode);

        if (EditorGUI.EndChangeCheck())
        {
            SwitchServer();
        }

        EditorGUILayout.LabelField("Server", ServerUrl, EditorStyles.miniLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Start Session"))
        {
            _ = StartSession();

        }
        
        // show error
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        //if session, show info
        if (currentSession != null)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Current Session", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Session ID", currentSession.sessionId);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("URL");

            if (EditorGUILayout.LinkButton(currentSession.sessionURL))
            {
                Application.OpenURL(currentSession.sessionURL);
            }

            EditorGUILayout.EndHorizontal();

            if (qrCode != null)
            {
                GUILayout.Space(10);
                GUILayout.Label("QR Code");

                GUILayout.Label(qrCode,
                GUILayout.Width(256),
                GUILayout.Height(256));
            }
        }

        GUILayout.Space(10);

        GUILayout.Label("Result:");

        EditorGUILayout.HelpBox(result, MessageType.Info);

        GUILayout.Space(10);
        GUILayout.Label("Scene Publishing", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(isPublishing))
        {
            string publishButtonLabel = isPublishing ? "Publishing..." : "Publish Active Scene";

            if (GUILayout.Button(publishButtonLabel))
            {
                _ = PublishActiveScene();
            }
        }

        if (!string.IsNullOrEmpty(publishedSceneUrl))
        {
            if (EditorGUILayout.LinkButton(publishedSceneUrl))
            {
                Application.OpenURL(publishedSceneUrl);
            }

            if (GUILayout.Button("Copy Scene URL"))
            {
                GUIUtility.systemCopyBuffer = publishedSceneUrl;
            }
        }

        // refresh button
        if (currentSession != null)
        {
            if (GUILayout.Button("Refresh Media"))
            {
                _ = RefreshMedia();
            }
        }

        foreach (Texture2D texture in media)
        {
        if (GUILayout.Button(
            texture,
            GUILayout.Width(128),
            GUILayout.Height(128)))
        {
            CreateSprite(texture);
        }
        }
    }

    private void SwitchServer()
    {
        client = new ToolkitServerClient(ServerUrl);
        currentSession = null;
        qrCode = null;
        media.Clear();
        errorMessage = "";
        result = "";
        publishedSceneUrl = "";
    }

    private async Awaitable StartSession()
    {
        errorMessage = "";
        currentSession = null;
        qrCode = null;

        currentSession = await client.StartSessionAsync();

        if (currentSession == null)
        {
            errorMessage = "Could not connect to Toolkit Server.";
            Repaint();
            return;
        }

        qrCode = await client.GetQRCodeAsync(currentSession);

        if (qrCode == null)
        {
            errorMessage = "Could not download QR code.";
        }

        Repaint();
    }

    private async Awaitable RefreshMedia()
    {
        media.Clear();

        List<string> files = await client.GetMediaAsync(currentSession);

        if (files == null)
        {
            return;
        }

        foreach (string file in files)
        {
            Texture2D texture =
                await client.DownloadMediaAsync(currentSession, file);

            if (texture != null)
            {
                texture.name = file;
                media.Add(texture);
            }
        }

        Repaint();
    }

    private async Awaitable PublishActiveScene()
    {
        errorMessage = "";
        result = "Building scene bundle...";
        publishedSceneUrl = "";

        if (!ToolkitSceneSerializer.TryBuildSceneBundle(out ToolkitSceneBundle bundle))
        {
            result = "Scene bundle could not be built.";
            Repaint();
            return;
        }

        isPublishing = true;
        result = "Publishing scene...";
        Repaint();

        ScenePublishResponse response = null;

        try
        {
            response = await client.PublishSceneAsync(bundle);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            isPublishing = false;
        }

        if (response == null || !response.success)
        {
            errorMessage = "Could not publish the scene. Check the Console and server logs.";
            result = "Scene publishing failed.";
            Repaint();
            return;
        }

        publishedSceneUrl = response.sceneUrl;
        result = "Scene published successfully.";
        Repaint();
    }

    private void CreateSprite(Texture2D texture)
    {
        GameObject obj = new GameObject("Toolkit Image");

        Undo.RegisterCreatedObjectUndo(obj, "Create Toolkit Image");

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        renderer.sprite = sprite;

        ToolkitSceneObject trackedObject = Undo.AddComponent<ToolkitSceneObject>(obj);
        string sourceAssetPath = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(sourceAssetPath))
        {
            sourceAssetPath = texture.name;
        }
        trackedObject.Initialize(sourceAssetPath);

        Bounds bounds = renderer.bounds;
        float size = Mathf.Max(bounds.size.x, bounds.size.y);

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minPlacementFraction, maxPlacementFraction) * size;

        float radius = Random.Range(
            minPlacementFraction,
            maxPlacementFraction
        ) * size;

        Vector3 position = Random.onUnitSphere * radius;

        // Optional: emphasize depth
        position.z *= depthMultiplier;

        obj.transform.position = position;

        obj.transform.position = position;

        obj.transform.rotation = Quaternion.Euler(
            Random.Range(-maxXRotation, maxXRotation),
            Random.Range(-maxYRotation, maxYRotation),
            Random.Range(-maxZRotation, maxZRotation)
        );

        Selection.activeGameObject = obj;

        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView != null)
        {
            sceneView.Frame(renderer.bounds, false);
        }
    }


}
