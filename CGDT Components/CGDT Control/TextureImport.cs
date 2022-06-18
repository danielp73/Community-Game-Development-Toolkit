using UnityEditor;
using UnityEngine;
using System.Collections;

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
            //if (EditorUtility.DisplayDialog("Import","Import image as Sprite to use as an image texture in a scene?", "Yes", "Do not make it a Sprite"))
            //{
            //    //set texture type to sprite
            //    textureImporter.textureType = TextureImporterType.Sprite;
            //}
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}