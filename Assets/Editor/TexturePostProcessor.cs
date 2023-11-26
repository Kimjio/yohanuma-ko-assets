using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        var importer = assetImporter as TextureImporter;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = false;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.GetDefaultPlatformTextureSettings().resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
    }
}
