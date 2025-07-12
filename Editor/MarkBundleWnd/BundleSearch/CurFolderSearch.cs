using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 打包当前文件夹
/// </summary>
public class CurFolderSearch : IBundleSearch
{
    public string[] SearchBundles(string folderPath)
    {
        return new string[] { folderPath };
    }
}
