using UnityEngine;
using UnityEditor;
using System.IO;

class MyClass
{
    [InitializeOnLoadMethod]
    static void OnProjectLoadedInEditor() 
    {

      
        Debug.Log("[CGDT Setup] Adding all scenes in project to build settings");

        //search for all scenes in project, add to build settings
        string[] files = Directory.GetFiles("Assets/", "*.unity", SearchOption.AllDirectories);
        foreach (string name in files)
        {
            AutoBuildScene.AddSceneToBuildSettings(name);
        }



    }
}