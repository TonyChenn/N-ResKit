using NCore.Editor;
using UnityEditor;
using UnityEngine;

//public enum BuildTargetEnum
//{
//    StandaloneWindows = BuildTarget.StandaloneWindows,
//    StandaloneWindows64 = BuildTarget.StandaloneWindows64,
//    StandaloneOSX = BuildTarget.StandaloneLinux64,
//    //StandaloneLinux64 = BuildTarget.StandaloneLinux64,
//    Android = BuildTarget.Android,
//    iOS = BuildTarget.iOS,
//}

public class BuildArguments : IEditorPrefs
{

    public BuildTarget CurBuildTarget
    {
        get { return EditorUserBuildSettings.activeBuildTarget; }
        set
        {
            if ((BuildTarget)value != EditorUserBuildSettings.activeBuildTarget)
            {
                EditorUtility.DisplayDialog("提示", "打包平台必须与激活平台保持一致", "好的");
            }
        }
    }
    /// <summary>
    /// 是否转表
    /// </summary>
    public bool ConvertTable
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_ConvertTable", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_ConvertTable", value, true); }
    }

    /// <summary>
    /// HybirdCLR
    /// </summary>
    public bool GenHybirdCLR
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_GenHybirdCLR", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_GenHybirdCLR", value, true); }
    }

    /// <summary>
    /// 打包工程
    /// </summary>
    public bool BuildProject
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_BuildProject", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_BuildProject", value, true); }
    }
    /// <summary>
    /// 全部重新打包（否则为增量打包）
    /// </summary>
    public bool BuildAll
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_BuildAll", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_BuildAll", value, true); }
    }

    /// <summary>
    /// 是否上传到服务器
    /// </summary>
    public bool UploadToFTP
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_UploadToFTP", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_UploadToFTP", value, true); }
    }

    /// <summary>
    /// 复制到StreammingAsset目录
    /// </summary>
    public bool CopyToSreeammingAsset
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_CopyToSreeammingAsset", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_CopyToSreeammingAsset", value, true); }
    }

    /// <summary>
    /// 打包UI
    /// </summary>
    public bool BuildUI
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_BuildUI", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_BuildUI", value, true); }
    }

    /// <summary>
    /// ExcelFolder
    /// </summary>
    public string ExcelFolder = "";

    #region IEditorPrefs
    public void ReleaseEditorPrefs()
    {
        EditorPrefsHelper.DeleteKey("BuildArguments_ConvertTable", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_GenHybirdCLR", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_BuildProject", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_BuildAll", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_UploadToFTP", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_CopyToSreeammingAsset", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_BuildUI", true);
    }
    #endregion
}