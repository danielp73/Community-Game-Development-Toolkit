using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ToolkitServerWindow : EditorWindow
{
    private const string LocalServerUrl = "http://localhost:8000";
    private const string HostedServerUrl = "https://share.communitygametoolkit.org";


    private ToolkitServerClient client;

    private const double PollInterval = 3.0;
    private readonly Dictionary<string, Texture2D> downloadedMedia = new();
    private readonly List<Texture2D> pendingMedia = new();
    private double nextMediaPoll;
    private bool isPollingMedia;
    private bool isStartingSession;
    private bool panelEnabled;
    private int sessionGeneration;
    private string mediaError;
    private Session currentSession;
    private Texture2D qrCode;
    private List<Texture2D> media = new();
    private string errorMessage;
    private bool isPublishing;
    private string publishedViewerUrl;
    
    //****** new sprite Placement

    private float minPlacementFraction = 0.5f;
    private float maxPlacementFraction = 2.0f;
    private float depthMultiplier = 2.0f;

    //******


    private string result = "";

    private string ServerUrl => ToolkitServerSettings.DevelopmentMode ? LocalServerUrl : HostedServerUrl;

    [MenuItem("Game-Toolkit/Add Media from Phone", false, 980)]
    [MenuItem("Game-Toolkit/Developer/Toolkit Server", false, 2004)]
    public static void ShowWindow()
    {
        // Unity's Inspector type is internal; resolve it only as a docking preference.
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = inspectorType != null
            ? GetWindow<ToolkitServerWindow>("Toolkit Server", true, inspectorType)
            : GetWindow<ToolkitServerWindow>("Toolkit Server");
        window.Show();
        window.Focus();
    }

    private void OnEnable()
    {
        panelEnabled = true;
        MergeMediaPreviews(media, pendingMedia, downloadedMedia);
        SetPreviewRetention(true);
        client = new ToolkitServerClient(ServerUrl);
        nextMediaPoll = 0;
        EditorApplication.update -= PollMedia;
        EditorApplication.update += PollMedia;
        ToolkitServerSettings.Changed -= OnServerSettingsChanged;
        ToolkitServerSettings.Changed += OnServerSettingsChanged;
    }

    private void OnDisable()
    {
        panelEnabled = false;
        SetPreviewRetention(false);
        sessionGeneration++;
        EditorApplication.update -= PollMedia;
        ToolkitServerSettings.Changed -= OnServerSettingsChanged;
    }

    private void SetPreviewRetention(bool retain)
    {
        SetTextureRetention(qrCode, retain);
        foreach (Texture2D texture in media) SetTextureRetention(texture, retain);
        foreach (Texture2D texture in pendingMedia) SetTextureRetention(texture, retain);
    }

    private static void SetTextureRetention(Texture2D texture, bool retain)
    {
        if (texture == null) return;
        if (retain) texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        else texture.hideFlags &= ~HideFlags.DontUnloadUnusedAsset;
    }

    private void OnServerSettingsChanged()
    {
        SwitchServer();
        Repaint();
    }

    private static void MergeMediaPreviews(List<Texture2D> visible, List<Texture2D> incoming,
        Dictionary<string, Texture2D> cache)
    {
        // Unity restores the texture list across script reloads, but not the dictionary.
        // Rebuild both together: a filename always corresponds to exactly one preview.
        var unique = new Dictionary<string, Texture2D>();
        var order = new List<string>();
        void Include(Texture2D texture)
        {
            if (texture == null || string.IsNullOrEmpty(texture.name)) return;
            string filename = texture.name;
            if (!unique.ContainsKey(filename)) order.Add(filename);
            unique[filename] = texture;
        }
        foreach (Texture2D texture in visible) Include(texture);
        foreach (Texture2D texture in incoming) Include(texture);
        visible.Clear();
        incoming.Clear();
        cache.Clear();
        foreach (string filename in order)
        {
            Texture2D texture = unique[filename];
            visible.Add(texture);
            cache.Add(filename, texture);
        }
    }

    private void OnGUI()
    {
        // Keep the same thumbnail controls for the complete Layout/Repaint cycle.
        if (Event.current.type == EventType.Layout)
        {
            MergeMediaPreviews(media, pendingMedia, downloadedMedia);
        }
        GUILayout.Label("Toolkit Server", EditorStyles.boldLabel);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Server", ServerUrl, EditorStyles.miniLabel);

        GUILayout.Space(10);

        using (new EditorGUI.DisabledScope(isStartingSession))
        {
            if (GUILayout.Button(isStartingSession ? "Starting Session..." : "Start Session"))
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

        if (!string.IsNullOrEmpty(publishedViewerUrl))
        {
            EditorGUILayout.LabelField("Viewer URL", EditorStyles.miniLabel);

            if (EditorGUILayout.LinkButton(publishedViewerUrl))
            {
                Application.OpenURL(publishedViewerUrl);
            }

            if (GUILayout.Button("Copy Viewer URL"))
            {
                GUIUtility.systemCopyBuffer = publishedViewerUrl;
            }
        }

        if (currentSession != null)
        {
            EditorGUILayout.LabelField("New media appears automatically.", EditorStyles.miniLabel);
            if (!string.IsNullOrEmpty(mediaError))
                EditorGUILayout.HelpBox(mediaError, MessageType.Warning);
        }

        foreach (Texture2D texture in media)
        {
            if (texture == null) continue;
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(150));
            EditorGUILayout.LabelField(texture.name, EditorStyles.miniLabel, GUILayout.Width(140));
            Rect preview = GUILayoutUtility.GetRect(128, 128, GUILayout.Width(140), GUILayout.Height(128));
            bool clicked = GUI.Button(preview, GUIContent.none);
            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawTextureTransparent(preview, texture, ScaleMode.ScaleToFit);
            if (clicked) CreateSprite(texture);
            EditorGUILayout.EndVertical();
        }
    }

    private void SwitchServer()
    {
        sessionGeneration++;
        SetPreviewRetention(false);
        downloadedMedia.Clear();
        pendingMedia.Clear();
        mediaError = null;
        client = new ToolkitServerClient(ServerUrl);
        currentSession = null;
        qrCode = null;
        media.Clear();
        errorMessage = "";
        result = "";
        publishedViewerUrl = "";
    }

    private bool IsCurrentSession(int generation) => panelEnabled && generation == sessionGeneration;

    private async Awaitable StartSession()
    {
        if (isStartingSession) return;
        isStartingSession = true;
        int generation = ++sessionGeneration;
        ToolkitServerClient sessionClient = client;
        errorMessage = "";
        mediaError = null;
        SetPreviewRetention(false);
        currentSession = null;
        qrCode = null;
        media.Clear();
        downloadedMedia.Clear();
        pendingMedia.Clear();
        try
        {
            Session session = await sessionClient.StartSessionAsync();
            if (!IsCurrentSession(generation)) return;
            if (session == null)
            {
                errorMessage = "Could not connect to Toolkit Server.";
                return;
            }
            currentSession = session;
            nextMediaPoll = 0;
            Texture2D code = await sessionClient.GetQRCodeAsync(session);
            if (!IsCurrentSession(generation))
            {
                if (code != null) DestroyImmediate(code);
                return;
            }
            qrCode = code;
            if (qrCode == null) errorMessage = "Could not download QR code.";
        }
        catch (System.Exception exception)
        {
            if (IsCurrentSession(generation)) errorMessage = exception.Message;
        }
        finally
        {
            isStartingSession = false;
            if (panelEnabled) Repaint();
        }
    }

    private static bool IsImageFile(string file)
    {
        if (string.IsNullOrWhiteSpace(file) || file.StartsWith(".") ||
            file.IndexOfAny(new[] { '/', '\\' }) >= 0) return false;
        string extension = System.IO.Path.GetExtension(file);
        return extension.Equals(".png", System.StringComparison.OrdinalIgnoreCase) ||
            extension.Equals(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
            extension.Equals(".jpeg", System.StringComparison.OrdinalIgnoreCase);
    }

    private void PollMedia()
    {
        if (!panelEnabled || currentSession == null || isPollingMedia ||
            EditorApplication.isCompiling || EditorApplication.isUpdating ||
            EditorApplication.timeSinceStartup < nextMediaPoll) return;
        _ = RefreshMedia();
    }

    private async Awaitable RefreshMedia()
    {
        isPollingMedia = true;
        int generation = sessionGeneration;
        Session session = currentSession;
        ToolkitServerClient sessionClient = client;
        try
        {
            List<string> files = await sessionClient.GetMediaAsync(session, true);
            if (!IsCurrentSession(generation)) return;
            if (files == null) throw new System.IO.IOException("Could not check for new media. Retrying automatically.");
            mediaError = null;
            foreach (string file in files)
            {
                if (!IsImageFile(file)) continue;
                if (downloadedMedia.TryGetValue(file, out Texture2D cached) && cached != null) continue;
                Texture2D texture = await sessionClient.DownloadMediaAsync(session, file, true);
                if (!IsCurrentSession(generation))
                {
                    if (texture != null) DestroyImmediate(texture);
                    return;
                }
                if (texture == null)
                {
                    mediaError = "Some media could not be downloaded. Retrying automatically.";
                    continue;
                }
                texture.name = file;
                downloadedMedia[file] = texture;
                pendingMedia.Add(texture);
            }
        }
        catch (System.Exception exception)
        {
            if (IsCurrentSession(generation))
            {
                string message = "Could not load new media: " + exception.Message + " Retrying automatically.";
                if (mediaError != message) Debug.LogWarning(message);
                mediaError = message;
            }
        }
        finally
        {
            isPollingMedia = false;
            nextMediaPoll = EditorApplication.timeSinceStartup + PollInterval;
            if (panelEnabled) Repaint();
        }
    }

    private async Awaitable PublishActiveScene()
    {
        errorMessage = "";
        result = "Building scene bundle...";
        publishedViewerUrl = "";

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

        // Show the playable viewer page rather than the underlying scene.json URL.
        string publishedSceneId = string.IsNullOrWhiteSpace(response.sceneId)
            ? bundle.sceneData.sceneId
            : response.sceneId.Trim();
        publishedViewerUrl = $"{ServerUrl}/viewer/?scene={publishedSceneId}";
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

        obj.transform.position = new Vector3(
            Mathf.Cos(angle) * distance,
            bounds.extents.y + 0.05f,
            Mathf.Sin(angle) * distance * depthMultiplier
        );
        obj.transform.rotation = Quaternion.identity;

        Selection.activeGameObject = obj;

        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView != null)
        {
            sceneView.Frame(renderer.bounds, false);
        }
    }


}
