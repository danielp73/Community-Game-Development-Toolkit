using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(SceneChange))]
public class SceneChangeEditor : Editor
{

    private SerializedProperty scenePathProp;
    private SerializedProperty isVisibleProp;
    private SerializedProperty sceneIndexProp;
    private int index;
    private string[] paths;
    private string exampleSceneCheck = "Community-Game-Development-Toolkit/Example Scenes";

    private void OnEnable()
    {
        Debug.Log("enable");
        scenePathProp = serializedObject.FindProperty("scenePath");
        isVisibleProp = serializedObject.FindProperty("isVisible");
        sceneIndexProp = serializedObject.FindProperty("sceneIndex");

        //paths = getScenePaths();
        //foreach (string theScene in paths)
        //    Debug.Log("found scene: " + theScene);



    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
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
        //get and store scene index
        sceneIndexProp.intValue = EditorGUILayout.Popup(sceneIndexProp.intValue, sceneNames);
        //store scene path, based on index
        scenePathProp.stringValue = paths[sceneIndexProp.intValue];
        Debug.Log("selected scene path: " + paths[sceneIndexProp.intValue]);

        EditorGUILayout.PropertyField(isVisibleProp);

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
