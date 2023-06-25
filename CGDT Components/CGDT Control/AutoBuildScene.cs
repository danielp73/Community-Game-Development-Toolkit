﻿//this script a slight modification of:
//https://gist.github.com/jbubriski/01474db14526318193a1733c6586acd3
//(dan)

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// docs: https://docs.unity3d.com/ScriptReference/AssetModificationProcessor.OnWillSaveAssets.html
public class AutoBuildScene : UnityEditor.AssetModificationProcessor
{

    private static List<string> ignorePaths = new List<string>();

    public static void OnWillCreateAsset(string path)
    {
        if (path.EndsWith(".unity.meta"))
            path = path.Substring(0, path.Length - 5);

        ProcessAssetsForScenes(new string[] { path });
    }

    public static string[] OnWillSaveAssets(string[] paths)
    {
        return ProcessAssetsForScenes(paths);
    }

    private static string[] ProcessAssetsForScenes(string[] paths)
    {
        string scenePath = string.Empty;

        foreach (string path in paths)
        {
            if (path.Contains(".unity"))
                scenePath = path;
        }

        // NOTE: add more logic here if you want to filter for certain paths.
        if (!string.IsNullOrEmpty(scenePath) && !ignorePaths.Contains(scenePath))
            AddSceneToBuildSettings(scenePath);

        // unity only saves the paths that you return here, so we always pass through everything we received.
        return paths;
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        // only bother adding scenes we don't have already.
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.path == scenePath)
                return;
        }

        Debug.Log("[CGDT] Added new scene: " + scenePath + " to build settings");

        EditorBuildSettingsScene newScene = new EditorBuildSettingsScene();
        newScene.path = scenePath;
        newScene.enabled = true;

        scenes.Add(newScene);
        EditorBuildSettings.scenes = scenes.ToArray();

        //if (EditorUtility.DisplayDialog(DIALOG_TITLE, DIALOG_MSG, DIALOG_OK, DIALOG_NO))
        //{
        //    EditorBuildSettingsScene newScene = new EditorBuildSettingsScene();
        //    newScene.path = scenePath;
        //    newScene.enabled = true;

        //    scenes.Add(newScene);
        //    EditorBuildSettings.scenes = scenes.ToArray();
        //}
        //else
        //{
        //    ignorePaths.Add(scenePath);
        //}
    }
}

#endif