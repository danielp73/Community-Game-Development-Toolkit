using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Text;

public class ToolkitServerClient
{
    private readonly string baseUrl;

    public ToolkitServerClient(string baseUrl)
    {
        this.baseUrl = baseUrl.TrimEnd('/');
    }

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

        Session session = JsonUtility.FromJson<Session>(request.downloadHandler.text);

        if (session != null)
        {
            session.sessionURL = $"{baseUrl}/session/{session.sessionId}";
        }

        return session;
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

    public async Awaitable<ScenePublishResponse> PublishSceneAsync(ToolkitSceneBundle bundle)
    {
        ScenePublishRequest publishData = new ScenePublishRequest
        {
            sceneJson = bundle.json
        };

        foreach (KeyValuePair<string, byte[]> image in bundle.images)
        {
            publishData.images.Add(new ScenePublishImage
            {
                fileName = image.Key,
                base64Data = Convert.ToBase64String(image.Value)
            });
        }

        string publishUrl = $"{baseUrl}/api/scenes/{bundle.sceneData.sceneId}/json";
        string requestJson = JsonUtility.ToJson(publishData);

        using var request = new UnityWebRequest(publishUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestJson));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"Publishing Toolkit scene to: {publishUrl}");
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(
                $"Scene publish failed ({request.responseCode}): {request.error}\n" +
                request.downloadHandler.text
            );
            return null;
        }

        ScenePublishResponse response = JsonUtility.FromJson<ScenePublishResponse>(
            request.downloadHandler.text
        );

        Debug.Log("Published Toolkit scene: " + response.sceneUrl);
        return response;
    }

    [Serializable]
    private class ScenePublishRequest
    {
        public string sceneJson;
        public List<ScenePublishImage> images = new List<ScenePublishImage>();
    }

    [Serializable]
    private class ScenePublishImage
    {
        public string fileName;
        public string base64Data;
    }

}
