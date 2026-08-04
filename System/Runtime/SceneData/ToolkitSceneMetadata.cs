using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ToolkitSceneMetadata : MonoBehaviour
{
    [SerializeField] private string sceneId;

    public string publishedName;

    public string SceneId
    {
        get
        {
            EnsureSceneId();
            return sceneId;
        }
    }

    public void Initialize(string defaultName)
    {
        EnsureSceneId();
        if (string.IsNullOrEmpty(publishedName))
        {
            publishedName = defaultName;
        }
    }

    private void Reset()
    {
        EnsureSceneId();
    }

    private void OnValidate()
    {
        EnsureSceneId();
    }

    private void EnsureSceneId()
    {
        if (string.IsNullOrEmpty(sceneId))
        {
            sceneId = Guid.NewGuid().ToString("N");
        }
    }
}
