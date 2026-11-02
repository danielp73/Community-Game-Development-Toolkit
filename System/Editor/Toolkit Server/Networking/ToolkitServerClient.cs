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

        request.timeout = 30;
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

        request.timeout = 30;
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return null;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        if (texture != null) texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return texture;
    }

    public async Awaitable<List<string>> GetMediaAsync(Session session, bool quiet = false)
    {
        using var request = UnityWebRequest.Get(
            $"{baseUrl}/api/session/{session.sessionId}/uploads"
        );

        request.timeout = 30;
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (!quiet) Debug.LogError(request.error);
            return null;
        }



        string json = request.downloadHandler.text.Trim();
        // Hosted and local server releases use different list shapes.
        if (json.StartsWith("[", StringComparison.Ordinal)) json = "{\"media\":" + json + "}";
        MediaList mediaList = JsonUtility.FromJson<MediaList>(json);
        if (mediaList?.media == null)
            throw new System.IO.InvalidDataException("The server returned an unrecognized media listing.");
        return mediaList.media;
    }

    public async Awaitable<Texture2D> DownloadMediaAsync(Session session, string filename, bool quiet = false)
    {
        string url = $"{baseUrl}/uploads/{Uri.EscapeDataString(session.sessionId)}/{Uri.EscapeDataString(filename)}";
        using var request = UnityWebRequest.Get(url);
        request.timeout = 30;
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            if (!quiet) Debug.LogError(request.error);
            return null;
        }

        // An HTTP success is not enough: don't cache a placeholder for an invalid/partial image.
        byte[] data = request.downloadHandler.data;
        if (data == null || data.Length == 0) return null;
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool decoded = false;
        try
        {
            decoded = ImageConversion.LoadImage(texture, data, false);
            if (decoded && texture.width > 0 && texture.height > 0)
            {
                // Editor scene changes may unload downloaded textures, which aren't asset files.
                texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
                return texture;
            }
            if (!quiet) Debug.LogWarning("Could not decode media image: " + filename);
            return null;
        }
        finally
        {
            if (!decoded) UnityEngine.Object.DestroyImmediate(texture);
        }
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
