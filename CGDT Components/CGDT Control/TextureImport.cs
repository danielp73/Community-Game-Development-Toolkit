#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class TextureImport : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        Debug.Log("Imported texture");

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        int startIndex = assetPath.LastIndexOf("/") + 1;
        string assetName = assetPath.Substring(startIndex, assetPath.Length - startIndex);
        if (assetName.EndsWith(".png") || assetName.EndsWith(".PNG") || assetName.EndsWith(".jpg") || assetName.EndsWith(".JPG") || assetName.EndsWith(".jpeg") || assetName.EndsWith(".JPEG"))
        {
            Debug.Log("CGDT: Imported image");
            //if (EditorUtility.DisplayDialog("Import", "Import image as Sprite to use as an image texture in a scene?", "Yes", "Do not make it a Sprite"))
            //{ 
            //    //set texture type to sprite
            //    textureImporter.textureType = TextureImporterType.Sprite;
            //}
            textureImporter.textureType = TextureImporterType.Sprite;
        }

        //int dotLocation = assetName.LastIndexOf(".");
        //string assetNameNoExtention = assetName.Substring(0, dotLocation);

        //if (assetName.Contains("360"))
        //{
        //    Debug.Log("CGDT: Imported Skybox Image");

        //    //insane way to load image file data into a texture
        //    Texture2D tex = new Texture2D(2,2); //2,2 is size but will be replaced by image size
        //    byte[] fileData;
        //    Debug.Log(assetPath);
        //    fileData = File.ReadAllBytes(assetPath);
        //    Debug.Log(fileData.Length);
        //    //bool success = tex.LoadImage(fileData);
        //    bool success = ImageConversion.LoadImage(tex, fileData);
        //    Debug.Log("LoadImage success: " + success + ". Height: " + tex.height + " Width: " + tex.width);

        //    //new material, set shader to skybox
        //    Material material = new Material(Shader.Find("Skybox/Panoramic"));

        //    //set 'main texture' which will be the Spherical HD data 
        //    material.mainTexture = tex;

      
        //    AssetDatabase.CreateAsset(material, "Assets/Community-Game-Development-Toolkit/Skyboxes - 360/" + assetNameNoExtention + "-SKYBOX.mat");
        //}
    }

    //IEnumerator CreateMat
}

#endif