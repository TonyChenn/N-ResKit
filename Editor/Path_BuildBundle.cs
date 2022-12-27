using NCore.Editor;
using UnityEditor;
using UnityEngine;

public class Path_BuildBundle : IPathConfig, IEditorPrefs
{
    private static string default_bundle_root_folder => $"{Application.dataPath}/BuildBundle";
    private static string default_bundle_database_folder => $"{Application.dataPath}/../AssetBundleDB";

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
    [SettingProperty(FieldType.EditField,"CDN 地址")]
    public static string CDNUrl
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_CDNUrl", default_bundle_cdn_url); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_CDNUrl", value); }
    }

    [SettingMethod("", "打开打包工具")]
    public static void OpenBuildBundleTool()
    {
        BuildWindow.ShowWindow();
    }

    public static int SelectedBuildPlaceIndex
    {
        get { return EditorPrefsHelper.GetInt("Path_BuildBundle_SelectedBuildPlaceIndex", 0); }
        set { EditorPrefsHelper.SetInt("Path_BuildBundle_SelectedBuildPlaceIndex", value); }
    }

    public static string BuildBundleFolderPath
    {
        get { return EditorPrefsHelper.GetString("Path_BuildBundle_BuildBundleFolderPath", Application.streamingAssetsPath); }
        set { EditorPrefsHelper.SetString("Path_BuildBundle_BuildBundleFolderPath", value); }
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
    }
    #endregion
}
