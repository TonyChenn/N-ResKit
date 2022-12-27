using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildBundleUtil
{
    /// <summary>
    /// 相对路径转绝对路径
    /// </summary>
    public static string GetFullPath(string relative_path)
    {
        return Application.dataPath + "/" + relative_path.Substring("Assets/".Length);
    }

    public static string GetRelativePath(string full_path)
    {
        return "Assets/" + full_path.Substring(Application.dataPath.Length + 1);
    }
}
