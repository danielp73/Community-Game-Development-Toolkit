using UnityEngine;
using UnityEngine.Networking;

public class ToolkitServerClient
{
    private const string baseUrl = "http://localhost:3000";

    public async Awaitable<Session> StartSessionAsync()
    {
        using var request = new UnityWebRequest(
            $"{baseUrl}/create-session",
            "POST"
        );

        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        return JsonUtility.FromJson<Session>(request.downloadHandler.text);
    }

    public async Awaitable<Texture2D> GetQRCodeAsync(Session session)
    {
        using var request = UnityWebRequestTexture.GetTexture(
            $"{baseUrl}/session/{session.sessionId}/qr"
        );

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        return DownloadHandlerTexture.GetContent(request);
    }

    public async Awaitable Test()
    {
        using var request = UnityWebRequest.Get("http://localhost:3000");

        await request.SendWebRequest();

        Debug.Log("Finished");
    }

}