using NCore.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class BuildBundlePrefs: IEditorPrefs
{
    public static int SelectedBuildPlaceIndex
    {
        get { return EditorPrefsHelper.GetInt("BuildBundlePrefs_SelectedBuildPlaceIndex", 0); }
        set { EditorPrefsHelper.SetInt("BuildBundlePrefs_SelectedBuildPlaceIndex", value); }
    }

    public static string BuildBundleFolderPath
    {
        get { return EditorPrefsHelper.GetString("BuildBundlePrefs_BuildBundleFolderPath", Application.streamingAssetsPath); }
        set { EditorPrefsHelper.SetString("BuildBundlePrefs_BuildBundleFolderPath", value); }
    }

    public void ReleaseEditorPrefs()
    {
        EditorPrefsHelper.DeleteKey("BuildBundlePrefs_SelectedBuildPlaceIndex");
        EditorPrefsHelper.DeleteKey("BuildBundlePrefs_BuildBundleFolderPath");
    }
}
