using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public abstract class AssetCategory
{
    private string m_outputFolder;
    private BaseBundle[] m_bundles;

    internal abstract BaseBundle[] GetAssetBundles();


    public string SrcFolder { get; private set; }
    public string Filter { get; private set; }

    public string OutputFolder
    {
        get { return m_outputFolder; }
        set { m_outputFolder = value.TrimStart('/').TrimEnd('/').ToLower(); }
    }

    internal BaseBundle[] Bundles
    {
        get
        {
			m_bundles ??= GetAssetBundles();

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
			List<AssetBundleBuild> items = new(Bundles.Length);
            foreach (var item in Bundles)
            {
                items.Add(new AssetBundleBuild()
                {
                    assetBundleName = item.AssetBundleName,
                    assetNames = item.AssetNames,
                });
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

    public virtual void OnAllBuildCompleted() { }

    public virtual void Dispose()
    {
		if(Bundles == null) return;

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
			string path = GetFullPath(folder);
            string[] files = Directory.GetFiles(path, _pattern, SearchOption.AllDirectories);
            string[] assets = new string[files.Length];
            for (int i = 0; i < assets.Length; i++)
			{
				assets[i] = GetRelativePath(files[i]);
			}
            return assets;
        }
        throw new ArgumentException("未知资源匹配模式");
    }

	/// <summary>
	/// 相对路径转绝对路径
	/// </summary>
	private static string GetFullPath(string relative_path)
	{
		return UnityEngine.Application.dataPath + "/" + relative_path.Substring("Assets/".Length);
	}

	private static string GetRelativePath(string full_path)
	{
		return "Assets/" + full_path.Substring(UnityEngine.Application.dataPath.Length + 1);
	}
	#endregion
}


internal abstract class BaseBundle
{
	string m_outputFolder;

	protected BaseBundle(string outputFolder)
	{
		m_outputFolder = outputFolder.TrimStart('/').TrimEnd('/');
	}
	public string AssetBundleName
	{
		get { return $"{m_outputFolder}/{Name}{Ext}"; }
	}

	public string FullName { get { return Name + Ext; } }


	#region absract

	public abstract string Name { get; }

	public abstract string[] AssetNames { get; }
	#endregion

	#region virtual
	public virtual void Dispose() { }
	public virtual string Ext { get { return ".u"; } }
	#endregion
}
