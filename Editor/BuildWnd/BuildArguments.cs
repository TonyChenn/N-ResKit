using NCore.Editor;
using System;
using UnityEditor;

public class BuildArguments : IEditorPrefs
{

    public BuildTarget CurBuildTarget
    {
        get { return EditorUserBuildSettings.activeBuildTarget; }
        set
        {
            if (value != EditorUserBuildSettings.activeBuildTarget)
            {
                EditorUtility.DisplayDialog("提示", "打包平台必须与激活平台保持一致", "好的");
            }
        }
    }

	/// <summary>
	/// 将bundle名以MD5命名
	/// </summary>
	public bool MD5BundleName
	{
		get { return EditorPrefsHelper.GetBool("BuildArguments_MD5BundleName", false, true); }
		set { EditorPrefsHelper.SetBool("BuildArguments_MD5BundleName", value, true); }
	}

    /// <summary>
    /// 是否上传到服务器
    /// </summary>
    public bool UploadToFTP
    {
        get { return EditorPrefsHelper.GetBool("BuildArguments_UploadToFTP", false, true); }
        set { EditorPrefsHelper.SetBool("BuildArguments_UploadToFTP", value, true); }
    }

    #region IEditorPrefs
    public void ReleaseEditorPrefs()
    {
		EditorPrefsHelper.DeleteKey("BuildArguments_MD5BundleName", true);
        EditorPrefsHelper.DeleteKey("BuildArguments_UploadToFTP", true);
    }
    #endregion
}
