using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ToolkitSceneObject : MonoBehaviour
{
    [SerializeField] private string objectId;

    public string objectType = "image";
    public string sourceAssetPath;
    public string assetUrl;
    public bool includeInPublishedScene = true;

    public string ObjectId
    {
        get
        {
            EnsureObjectId();
            return objectId;
        }
    }

    public void Initialize(string sourcePath, string sourceUrl = "")
    {
        EnsureObjectId();
        objectType = "image";
        sourceAssetPath = sourcePath;
        assetUrl = sourceUrl;
        includeInPublishedScene = true;
    }

    private void Reset()
    {
        EnsureObjectId();
    }

    private void OnValidate()
    {
        EnsureObjectId();
    }

    private void EnsureObjectId()
    {
        if (string.IsNullOrEmpty(objectId))
        {
            objectId = Guid.NewGuid().ToString("N");
        }
    }
}
