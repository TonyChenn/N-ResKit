using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FolderHelper
{
    public static void CreateFileFolder(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            string folder = Path.GetDirectoryName(filePath);
            CreateFolderIfNotExist(folder);
        }
    }

    public static void CreateFolderIfNotExist(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }
}
