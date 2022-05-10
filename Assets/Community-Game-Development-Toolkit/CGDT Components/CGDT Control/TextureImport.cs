using UnityEditor;
using UnityEngine;
using System.Collections;

public class TextureImport : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        Debug.Log("Imported texture");

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        //textureImporter.maxTextureSize = 512;
        //textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;

        int startIndex = assetPath.LastIndexOf("/") + 1;
        string assetName = assetPath.Substring(startIndex, assetPath.Length - startIndex);
        if (assetName.EndsWith(".png") || assetName.EndsWith(".PNG"))
        {
            Debug.Log("CGDT: Imported PNG");
            //set texture type to sprite
            textureImporter.textureType = TextureImporterType.Sprite;
        }
        if (assetName.EndsWith(".jpg") || assetName.EndsWith(".JPG"))
        {
            Debug.Log("CGDT: Imported JPG");
            //set texture type to sprite
            textureImporter.textureType = TextureImporterType.Sprite;
        }
        if (assetName.EndsWith(".jpeg") || assetName.EndsWith(".JPEG"))
        {
            Debug.Log("CGDT: Imported JPEG");
            //set texture type to sprite
            textureImporter.textureType = TextureImporterType.Sprite;
        }

        //    {
        //        textureImporter.textureType = TextureImporterType.Sprite;
        //    }


        //    if (assetName.StartsWith("s_"))
        //    {
        //        textureImporter.textureType = TextureImporterType.Sprite;
        //    }
        //    else if (assetName.StartsWith("t_"))
        //    {
        //        textureImporter.textureType = TextureImporterType.Default;
        //    }
        //    else if (assetName.StartsWith("n_"))
        //    {
        //        textureImporter.convertToNormalmap = true;
        //    }
    }
}