using NCore;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class MD5Helper
{
    private static MD5 md5 = null;
    private static StringBuilder builder = null;

    private static MD5 Md5
    {
        get
        {
            if (md5 == null)
                md5 = MD5.Create();
            return md5;
        }
    }
    private static StringBuilder Builder
    {
        get
        {
            if (builder == null)
                builder = new StringBuilder();
            return builder;
        }
    }

    #region 计算Hash API
    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string ComputeStringHash(string source)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(source);
        return ComputeHash(buffer);
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string ComputeHash(string assetPath)
    {
        byte[] buffer = ReadFileBytes(assetPath);
        return buffer != null ? ComputeHash(buffer) : null;
    }

    /// <summary>
    /// 计算单个文件联合依赖项的哈希码
    /// </summary>
    public static string ComputeHashWithDependencies(string assetPath)
    {
        byte[] buffer = ReadFileWithDependenciesBytes(assetPath);
        return ComputeHash(buffer);
    }

    /// <summary>
    /// 计算多个文件的MD5值
    /// </summary>
    public static string ComputeHash(string[] assetPathArray)
    {
        List<byte> list = new List<byte>();
        foreach (string assetPath in assetPathArray)
        {
            byte[] buffer = ReadFileBytes(assetPath);
            if (buffer != null)
                list.AddRange(buffer);
        }

        // 依赖项
        string[] dependencies = AssetDatabase.GetDependencies(assetPathArray);
        foreach (string dependency in dependencies)
        {
            byte[] bufferOfD = ReadFileBytes(dependency);
            if (bufferOfD != null)
                list.AddRange(bufferOfD);
        }

        return ComputeHash(list.ToArray());
    }

    /// <summary>
    /// 计算若干个文件合并成的哈希码
    /// </summary>
    public static string ComputeHashWithDependencies(string[] assetPaths)
    {
        List<byte> list = new List<byte>();
        foreach (string assetPath in assetPaths)
        {
            byte[] buffer = ReadFileBytes(assetPath);
            if (buffer != null)
                list.AddRange(buffer);
        }

        // 依赖项
        string[] dependencies = AssetDatabase.GetDependencies(assetPaths);
        foreach (string dependency in dependencies)
        {
            byte[] bufferOfD = ReadFileBytes(dependency);
            if (bufferOfD != null)
                list.AddRange(bufferOfD);
        }
        return ComputeHash(list.ToArray());
    }

    public static string ComputeHash(byte[] buffer)
    {
        if (buffer == null || buffer.Length < 1) return null;

        var builder = StringBuilderPool.Alloc();
        byte[] hash = Md5.ComputeHash(buffer);
        foreach (var b in hash)
        {
            string temp = b.ToString("x2");
            builder.Append(temp);

        }

        string result = builder.ToString();
        builder.Recycle();
        return result;
    }
    #endregion


    static byte[] ReadFileBytes(string assetPath)
    {
        string fullPath = Application.dataPath + assetPath.Substring(6);
        if (!File.Exists(fullPath)) return null;

        List<byte> list = new List<byte>();
        byte[] data = File.ReadAllBytes(fullPath);
        list.AddRange(data);

        string metaPath = fullPath + ".meta";
        byte[] dataMeta = File.ReadAllBytes(metaPath);
        list.AddRange(dataMeta);

        return list.ToArray();
    }
    static byte[] ReadFileWithDependenciesBytes(string assetPath)
    {
        byte[] buffer = ReadFileBytes(assetPath);
        if (buffer == null) return null;

        List<byte> result = new List<byte>();
        // 依赖项
        string[] dependencies = AssetDatabase.GetDependencies(new string[] { assetPath });
        foreach (var dependency in dependencies)
        {
            byte[] bufferOfD = ReadFileBytes(dependency);
            if (bufferOfD != null)
                result.AddRange(bufferOfD);
        }

        return result.ToArray();
    }
}