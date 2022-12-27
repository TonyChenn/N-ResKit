using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class AssetCategory
{
    private string m_outputFolder;
    private BaseBundle[] m_bundles;

    public abstract BaseBundle[] GetAssetBundles();


    public string SrcFolder { get; private set; }
    public string Filter { get; private set; }

    public string OutputFolder
    {
        get { return m_outputFolder; }
        set { m_outputFolder = value.TrimStart('/').TrimEnd('/').ToLower(); }
    }

    public bool HasChangedItem
    {
        get
        {
            if (Bundles != null)
            {
                foreach (var item in Bundles)
                {
                    if (item.LastHash != item.CurrentHash)
                        return true;
                }
            }
            return false;
        }
    }

    public BaseBundle[] Bundles
    {
        get
        {
            if (m_bundles == null) m_bundles = GetAssetBundles();

            return m_bundles;
        }
    }

    public AssetCategory(string srcFolder, string filter, string outputFolder)
    {
        SrcFolder = srcFolder;
        Filter = filter;
        OutputFolder = outputFolder;
    }

    public List<AssetBundleBuild> AssetBundleBuilds
    {
        get
        {
            List<AssetBundleBuild> items = new List<AssetBundleBuild>();
            foreach (var item in Bundles)
            {
                if (item.NeedBuild)
                {
                    items.Add(new AssetBundleBuild()
                    {
                        assetBundleName = item.AssetBundleName,
                        assetNames = item.AssetNames,
                    });
                }
            }
            return items;
        }
    }

    protected string[] GetAssets()
    {
        return GetAssets(SrcFolder, Filter);
    }




    public override string ToString()
    {
        return $"{SrcFolder}\t{Filter}\t{OutputFolder}";
    }

    #region virtual method
    public virtual void PrepareBuild() { }

    public virtual void OnBuildFinished() { }

    public virtual void OnBeforeComputeHash() { }

    public virtual void ComputeHash()
    {
        for (int i = 0, iMax = Bundles.Length; i < iMax; i++)
        {
            Bundles[i].ComputeHashIfNeed();
        }
    }

    public virtual void OnAllBuildCompleted() { }

    public virtual void Dispose()
    {
        for (int i = 0, iMax = Bundles.Length; i < iMax; i++)
        {
            Bundles[i].Dispose();
        }
    }
    #endregion

    #region static

    /// <summary>
    /// 获取原始文件
    /// </summary>
    public static string[] GetAssets(string folder, string filter)
    {
        if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(filter))
            throw new ArgumentException("folder/filter");

        if (filter.StartsWith("t:"))
        {
            string[] guids = AssetDatabase.FindAssets(filter, new string[] { folder });
            string[] assets = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                assets[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            return assets;
        }

        if (filter.StartsWith("f:"))
        {
            string _pattern = filter.Substring(2);
            string path = BuildBundleUtil.GetFullPath(folder);
            string[] files = Directory.GetFiles(path, _pattern, SearchOption.AllDirectories);
            string[] assets = new string[files.Length];
            for (int i = 0; i < assets.Length; i++)
                assets[i] = BuildBundleUtil.GetRelativePath(files[i]);
            return assets;
        }
        throw new ArgumentException("未知资源匹配模式");
    }
    #endregion
}
