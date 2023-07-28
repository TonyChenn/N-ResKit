﻿using NCore.Editor;
using UnityEditor;
using UnityEngine;

public class Path_BuildBundle : IPathConfig, IEditorPrefs
{
    private static string default_bundle_root_folder => $"{Application.dataPath}/BuildBundle";
    private static string default_bundle_database_folder => $"{Application.dataPath}/../../AssetBundleDB";

    private static BuildAssetBundleOptions default_bundle_compression = BuildAssetBundleOptions.ChunkBasedCompression;

    private static string default_bundle_cdn_url = "http://localhost";

    [SettingProperty(FieldType.Folder,"Bundle根目录")]
    public static string BundleRootFolder
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_BundleRootFolder", default_bundle_root_folder); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_BundleRootFolder", value); }
    }

    [SettingProperty(FieldType.Folder,"Bundle库目录")]
    public static string BundleDBFolder
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_BundleDBFolder", default_bundle_database_folder); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_BundleDBFolder", value); }
    }

    [SettingProperty(FieldType.Enum, "压缩方式")]
    public static BuildAssetBundleOptions BundleCompression
    {
        get { return EditorPrefsHelper.GetEnum("Path_BuildBundle_CompressionOption", default_bundle_compression); }
        set { EditorPrefsHelper.SetEnum("Path_BuildBundle_CompressionOption", value); }
    }

    [SettingProperty(FieldType.EditField,"CDN 地址(暂时废弃)")]
    public static string CDNUrl
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_CDNUrl", default_bundle_cdn_url); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_CDNUrl", value); }
    }

    [SettingProperty(FieldType.Toggle, "剔除通用字体")]
    public static bool EnableRemoveCommonFont
    {
        get { return EditorPrefsHelper.GetBool("Path_BuildBundle_EnableRemoveCommonFont", false); }
        set { EditorPrefsHelper.SetBool("Path_BuildBundle_EnableRemoveCommonFont", value); }
    }

    [SettingProperty(FieldType.Toggle, "剔除通用图集")]
    public static bool EnableRemoveCommonAtlas
    {
        get { return EditorPrefsHelper.GetBool("Path_BuildBundle_EnableRemoveCommonAtlas", false); }
        set { EditorPrefsHelper.SetBool("Path_BuildBundle_EnableRemoveCommonAtlas", value); }
    }

    [SettingMethod("", "打开打包工具")]
    public static void OpenBuildBundleTool()
    {
        BuildWindow.ShowWindow();
    }


    /// <summary>
    /// BuildWindow中选择输出路径的索引
    /// </summary>
    public static int SelectedBuildPlaceIndex
    {
        get { return EditorPrefsHelper.GetInt("Path_BuildBundle_SelectedBuildPlaceIndex", 0); }
        set { EditorPrefsHelper.SetInt("Path_BuildBundle_SelectedBuildPlaceIndex", value); }
    }

    /// <summary>
    /// BuildWindow真正build bundle输出的目录
    /// </summary>
    public static string BuildBundleFolderPath
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_BuildBundleFolderPath", Application.streamingAssetsPath); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_BuildBundleFolderPath", value); }
    }

    [MenuItem("Tools/Open../Bundle库")]
    private static void OpenBundleDatabaseFolder()
    {
        if (System.IO.Directory.Exists(BundleDBFolder))
        {
            Application.OpenURL($"file:///{BundleDBFolder}");
        }
        else
        {
            if(EditorUtility.DisplayDialog("提示","AB包库目录不存在","去设置"))
            {
                PackageWnd.ShowWnd(SettingPage.TAG, "Build Bundle");
            }
        }
    }

    #region IPathConfig, IEditorPrefs
    public const string TAG = "Build Bundle";
    public string GetModuleName()
    {
        return TAG;
    }

    public void ReleaseEditorPrefs()
    {
        EditorPrefs.DeleteKey("Path_BuildBundle_BundleRootFolder");
        EditorPrefs.DeleteKey("Path_BuildBundle_BundleDBFolder");

        EditorPrefsHelper.DeleteKey("Path_BuildBundle_SelectedBuildPlaceIndex");
        EditorPrefsHelper.DeleteKey("Path_BuildBundle_BuildBundleFolderPath");
        EditorPrefsHelper.DeleteKey("Path_BuildBundle_CDNUrl");
        EditorPrefsHelper.DeleteKey("Path_BuildBundle_EnableRemoveCommonFont");
        EditorPrefsHelper.DeleteKey("Path_BuildBundle_EnableRemoveCommonAtlas");
    }
    #endregion
}
