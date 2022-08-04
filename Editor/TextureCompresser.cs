using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureCompresser : AssetPostprocessor
{
    //void OnPreprocessTexture()
    //{
    //    string msg = string.Format("是否压缩图片？路径：{0}", assetImporter.assetPath);
    //    bool yes = EditorUtility.DisplayDialog("图片压缩", msg, "是", "否");
    //    if (yes)
    //    {
    //        TextureImporter importer = (TextureImporter)assetImporter;
    //        CompressTexture(importer);
    //    }
    //}

    static bool CompressTexture(TextureImporter importer)
    {
        bool rgba = importer.DoesSourceTextureHaveAlpha();
        if (rgba)
            return CompressRGBA(importer);
        else
            return CompressRGB(importer);
    }

    static bool CompressRGB(TextureImporter importer)
    {
        return SetSettings(importer, 2048, TextureImporterFormat.ETC2_RGB4, TextureImporterFormat.ASTC_4x4);
    }

    public static bool CompressRGBA(string assetPath)
    {
        return SetSettings(assetPath, 2048, TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ASTC_4x4);
    }

    static bool CompressRGBA(TextureImporter importer)
    {
        return SetSettings(importer, 2048, TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ASTC_4x4);
    }

    static bool SetSettings(string assetPath, int maxSize, TextureImporterFormat androidFormat, TextureImporterFormat iosFormat)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError(string.Format("compress texture failed, path: {0}", assetPath));
            return false;
        }

        return SetSettings(importer, maxSize, androidFormat, iosFormat);
    }

    static bool SetSettings(TextureImporter importer, int maxSize, TextureImporterFormat androidFormat, TextureImporterFormat iosFormat)
    {
        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        bool changed = false;

        if (settings.npotScale != TextureImporterNPOTScale.ToNearest)
        {
            settings.npotScale = TextureImporterNPOTScale.ToNearest;
            changed = true;
        }

        if (settings.readable != false)
        {
            settings.readable = false;
            changed = true;
        }

        if (settings.mipmapEnabled != false)
        {
            settings.mipmapEnabled = false;
            changed = true;
        }

        if (settings.alphaIsTransparency != true)
        {
            settings.alphaIsTransparency = true;
            changed = true;
        }

        if (settings.wrapMode != TextureWrapMode.Clamp)
        {
            settings.wrapMode = TextureWrapMode.Clamp;
            changed = true;
        }

        if (changed)
            importer.SetTextureSettings(settings);

        if (importer.maxTextureSize != maxSize)
        {
            importer.maxTextureSize = maxSize;
            changed = true;
        }

        if (importer.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            changed = true;
        }

        var androidSetting = importer.GetPlatformTextureSettings("Android");
        if (androidSetting.format != androidFormat)
        {
            androidSetting.format = androidFormat;
            changed = true;
        }
        if (androidSetting.compressionQuality != 1)
        {
            androidSetting.compressionQuality = 1;
            changed = true;
        }
        importer.SetPlatformTextureSettings(androidSetting);

        var iosSetting = importer.GetPlatformTextureSettings("iPhone");
        if (iosSetting.format != iosFormat)
        {
            iosSetting.format = iosFormat;
            changed = true;
        }
        if (iosSetting.compressionQuality != 1)
        {
            iosSetting.compressionQuality = 1;
            changed = true;
        }
        importer.SetPlatformTextureSettings(iosSetting);

        var pcSetting = importer.GetPlatformTextureSettings("Standalone");
        if (pcSetting.format != TextureImporterFormat.ARGB32)
        {
            pcSetting.format = TextureImporterFormat.ARGB32;
            changed = true;
        }
        if (pcSetting.compressionQuality != 1)
        {
            pcSetting.compressionQuality = 1;
            changed = true;
        }
        importer.SetPlatformTextureSettings(pcSetting);

        if (changed)
        {
            AssetDatabase.ImportAsset(importer.assetPath);
            Debug.Log(string.Format("Compress rgba finished, path: {0}", importer.assetPath));
            return true;
        }
        return false;
    }
}
