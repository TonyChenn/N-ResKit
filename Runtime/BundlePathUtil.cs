using System.IO;
using N_AssetBundle.Runtime;
using NCore;
using UnityEngine;

public static class BundlePathUtil
{
    private static string temp_str = string.Empty;
    
    /// <summary>
    /// 获取当前平台名称
    /// </summary>
    public static string CurPlatformName
    {
        get
        {
#if UNITY_EDITOR
            return GetEditorBuildPlatformName(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
            return GetRuntimePlatformName(Application.platform);
#endif
        }
    }

#if UNITY_EDITOR
    private static string GetEditorBuildPlatformName(UnityEditor.BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case UnityEditor.BuildTarget.StandaloneWindows:
            case UnityEditor.BuildTarget.StandaloneWindows64:
                return "Windows";
            case UnityEditor.BuildTarget.Android:
                return "Android";
            case UnityEditor.BuildTarget.iOS:
                return "IOS";
#if UNITY_2019_2_OR_NEWER
            case UnityEditor.BuildTarget.StandaloneLinux64:
#else
                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
#endif
                return "Linux";
            case UnityEditor.BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return null;
        }
    }
#endif

    static string GetRuntimePlatformName(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            case RuntimePlatform.LinuxPlayer:
                return "Linux";
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
    }

    /// <summary>
    /// Res资源版本配置文件
    /// </summary>
    public static string VersionFilePath
    {
        get
        {
            return null;
            // if (HotfixMgr.Singlton.UpdateState == UpdateState.Nerver)
            //     return GetStreammingAssetBundlePath(VersionFileName);
            // else
            //     return GetPersistentAssetBundlesPath(VersionFileName);
        }
    }

    /// <summary>
    /// 获取AB包路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetAssetBundlePath(string path)
    {
        string fullPath = string.Empty;
        if (!ResManager.UseLocalAsset)
        {
            fullPath = GetAssetBundlePersistPath(path);
            if (File.Exists(fullPath))
                return fullPath;
        }
        return GetAssetBundleStreammingPath(path);
    }

    #region 
    
    /// <summary>
    /// 从Persistent获取资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetAssetBundlePersistPath(string path)
    {
        var builder = StringBuilderPool.Alloc();
        builder.Append(Application.persistentDataPath);
        builder.Append("/");
        builder.Append(path);
        temp_str = builder.ToString();
        builder.Recycle();
        return temp_str;
    }

    private static string GetAssetBundleStreammingPath(string path)
    {
        var builder = StringBuilderPool.Alloc();
        builder.Append(Application.streamingAssetsPath);
        builder.Append("/");
        builder.Append(path);
        temp_str = builder.ToString();
        builder.Recycle();
        return temp_str;
    }
    #endregion

}