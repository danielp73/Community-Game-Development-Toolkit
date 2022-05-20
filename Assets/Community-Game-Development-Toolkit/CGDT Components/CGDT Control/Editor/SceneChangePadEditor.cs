using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(SceneChangePad))]
public class SceneChangePadEditor : Editor
{

    private SerializedProperty scenePath;
    private int index;
    private string[] paths;
    private string exampleSceneCheck = "Community-Game-Development-Toolkit/Example Scenes";
    //private string[] sceneNames;

    private void OnEnable()
    {
        Debug.Log("enable");
        scenePath = serializedObject.FindProperty("scenePath");

        paths = getScenePaths();
        foreach (string theScene in paths)
            Debug.Log("found scene: " + theScene);

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.PropertyField(scenePath);
        paths = getScenePaths();
        string[] sceneNames = new string[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            sceneNames[i] = Path.GetFileNameWithoutExtension(paths[i]);
            if (paths[i].Contains(exampleSceneCheck))
            {
                sceneNames[i] = "[CGDT Example Scene] " + sceneNames[i];
            }
        }
        index = EditorGUILayout.Popup(index, sceneNames);
        scenePath.stringValue = paths[index];
        Debug.Log("selected scene path: " + paths[index]);
        serializedObject.ApplyModifiedProperties();
    }

    private string [] getScenePaths()
    {
        var guids = AssetDatabase.FindAssets("t:Scene");
        string [] thePaths = Array.ConvertAll<string, string>(guids, AssetDatabase.GUIDToAssetPath);
        thePaths = Array.FindAll(thePaths, File.Exists); // Unity erroneously considers folders named something.unity as scenes, remove them
        return thePaths;
    }


}
