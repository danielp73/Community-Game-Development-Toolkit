using UnityEditor;
using UnityEngine;

public class ToolkitServerWindow : EditorWindow
{
    private ToolkitServerClient client;

    private Session currentSession;
    private Texture2D qrCode;

    private string result = "";

    [MenuItem("Game-Toolkit/Developer/Toolkit Server")]
    public static void ShowWindow()
    {
        GetWindow<ToolkitServerWindow>("Toolkit Server");
    }

    private void OnEnable()
    {
        client = new ToolkitServerClient();
    }

    private void OnGUI()
    {
        GUILayout.Label("Toolkit Server", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Start Session"))
        {
            _ = StartSession();

        }

        if (currentSession != null)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Current Session", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Session ID", currentSession.sessionId);

            EditorGUILayout.LabelField("URL", currentSession.sessionURL);

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
    }

    private async Awaitable StartSession()
    {
        currentSession = await client.StartSessionAsync();

        if (currentSession != null)
        {
            qrCode = await client.GetQRCodeAsync(currentSession);
        }

        Repaint();
    }

}