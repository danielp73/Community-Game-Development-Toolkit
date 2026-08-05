using System;
using System.Collections.Generic;

[Serializable]
public class ToolkitSceneData
{
    public string sceneId;
    public string title;
    public ToolkitPlayerData player;
    public List<ToolkitObjectData> objects = new List<ToolkitObjectData>();
}
