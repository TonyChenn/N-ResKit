using System;
using System.IO;

public class VersionHelper
{
    private const string VERSION_FILE_NAME = "version.data";
    private const string VERSION_FILE_TEMP_NAME = "version.data.temp";

    public static string GetRemoteVersion
    {
        get
        {
            return "";
        }
    }

    /// <summary>
    /// 获取本地版本文件
    /// </summary>
    public static string GetLocalVersion(string rootPath)
    {
        string path = $"{rootPath}/{VERSION_FILE_NAME}";
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return "0";
    }

    public static string GenNewVersionString()
    {
        return DateTime.Now.ToString("yyyy.MM.dd.hh.mm");
    }
}