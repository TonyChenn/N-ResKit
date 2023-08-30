using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 打包所有子文件
/// </summary>
public class SubFilesSearch : IBundleSearch
{
    public string[] SearchBundles(string folderPath)
    {
        List<string> result = new List<string>();
        SearchHandler(folderPath, ref result);

        return result.ToArray();
    }

    private void SearchHandler(string folderPath, ref List<string> result)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            result.Add(file.FullName);
        }

        foreach (var item in directoryInfo.GetDirectories())
        {
            SearchHandler(item.FullName, ref result);
        }
    }
}
