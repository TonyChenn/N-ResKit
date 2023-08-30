using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 搜集子文件夹(每个子文件夹打包)
/// </summary>
public class SubFolderSearch : IBundleSearch
{
    public string[] SearchBundles(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return null;

        DirectoryInfo[] array = new DirectoryInfo(folderPath).GetDirectories();

        string[] result = new string[array.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = array[i].FullName;
        }

        return result;
    }
}
