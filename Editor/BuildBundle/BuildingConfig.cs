using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UObject = UnityEngine.Object;

public static class BuildingConfig
{
    /// <summary>
    /// UIPrefab/字体/图集/散图
    /// </summary>
    /// <returns></returns>
    public static List<AssetCategory> GetBuildUIConfig()
    {
        return new List<AssetCategory>()
        {
            new UIPrefabs("Assets/BuildBundle/UI/Prefabs","ui/prefabs"),
            new NormalAssets<UObject>("Assets/BuildBundle/UI/Font","f:*.ttf","ui/font"),
            new NormalAssets<Texture>("Assets/BuildBundle/UI/Image","t:Texture2D","ui/image"),
        };
    }

    /// <summary>
    /// config/table
    /// </summary>
    /// <returns></returns>
    public static List<AssetCategory> GetBuildAssetConfig()
    {
        return new List<AssetCategory>()
        {
            //new NormalAssets<UObject>("Assets/BuildBundle/Asset/Config","f:*.asset","asset/config"),
            new NormalAssets<UObject>("Assets/BuildBundle/Asset/Table","f:*.asset","asset/table"),
        };
    }

    public static List<AssetCategory> GetAllConfig()
    {
        return new List<AssetCategory>()
        {
            // UIPrefab/字体/图集/散图
            new UIPrefabs("Assets/BuildBundle/UI/Prefabs","ui/prefabs"),
            new NormalAssets<UObject>("Assets/BuildBundle/UI/Font","f:*.ttf","ui/font"),
            new NormalAssets<Texture>("Assets/BuildBundle/UI/Image","t:Texture2D","ui/image"),


            // config/table
            new NormalAssets<UObject>("Assets/BuildBundle/Asset/Table","f:*.asset","asset/table"),


            // DLL(无需打包)
            //new DLLCodes($"Assets/BuildBundle/DLL/{EditorUserBuildSettings.activeBuildTarget}", "dll"),

            // Scenes
            new NormalAssets<UObject>("Assets/BuildBundle/Scenes","t:Scene","scenes"),

            // Sounds
            new NormalAssets<AudioClip>("Assets/BuildBundle/Sounds","t:AudioClip","sounds"),

            // video
            new NormalAssets<VideoClip>("Assets/BuildBundle/Videos","t:VideoClip","video"),

        };
    }
}
