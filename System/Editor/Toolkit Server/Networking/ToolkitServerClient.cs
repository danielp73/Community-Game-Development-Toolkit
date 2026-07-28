using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

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

    public async Awaitable<List<string>> GetMediaAsync(Session session)
    {
        using var request = UnityWebRequest.Get(
            $"{baseUrl}/api/session/{session.sessionId}/uploads"
        );

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        Debug.Log("media list: " + request.downloadHandler.text);

        MediaList mediaList = JsonUtility.FromJson<MediaList>(
            request.downloadHandler.text
        );

        return mediaList.media;
    }

    public async Awaitable<Texture2D> DownloadMediaAsync(Session session, string filename)
    {
        string url = $"{baseUrl}/uploads/{session.sessionId}/{filename}";

        using var request = UnityWebRequestTexture.GetTexture(url);
 
        Debug.Log("requesting: " + url);
        
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        } else
        {
            
        }

        return DownloadHandlerTexture.GetContent(request);
    }

}