using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UObject = UnityEngine.Object;

public static class BuildingConfig
{
    public static List<AssetCategory> GetConfig()
    {
        return new List<AssetCategory>()
        {
            // Asset
            new NormalAssets<UObject>("Assets/BuildBundle/Asset/Config","f:*.asset","asset/config"),
            new NormalAssets<UObject>("Assets/BuildBundle/Asset/Table","f:*.asset","asset/table"),

            // DLL
            new DLLCodes($"Assets/BuildBundle/DLL/{EditorUserBuildSettings.activeBuildTarget}", "dll"),

            // Scenes
            new NormalAssets<UObject>("Assets/BuildBundle/Scenes","f:*.unity","scenes"),

            // Sounds
            new NormalAssets<AudioClip>("Assets/BuildBundle/Sounds","t:AudioClip","sounds"),

            // UI
            //new SingleFolder("Assets/BuildBundle/UI/Atlas","ui/atlas"),
            new NormalAssets<UObject>("Assets/BuildBundle/UI/Font","f:*.ttf","ui/font"),
            new NormalAssets<Texture>("Assets/BuildBundle/UI/Image","t:Texture2D","ui/image"),

            // video
            new NormalAssets<VideoClip>("Assets/BuildBundle/Videos","t:VideoClip","video"),

        };
    }
}
