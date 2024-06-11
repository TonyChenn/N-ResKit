using NCore;
using UnityEngine;


public static class PathUtil
{
	private static string temp_str = string.Empty;

	/// <summary>
	/// 如果Persistent中存在返回，不存在则返回Streamming中
	/// </summary>
	public static string GetBundlePath(string bundleName, bool isFolder = false)
	{
		if (GameConfig.PlayMode == PlayMode.OfflineMode)
		{
			return GetBundleStreammingPath(bundleName, isFolder);
		}
		else if (GameConfig.PlayMode == PlayMode.HostMode)
		{
			string path = GetBundlePersistPath(bundleName, isFolder);
			if (System.IO.File.Exists(path))
				return path;

			return GetBundleStreammingPath(bundleName, isFolder);
		}
		else
		{
			return GetBundleStreammingPath(bundleName, isFolder);
		}
	}
	/// <summary>
	/// 从Persistent获取资源
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static string GetBundlePersistPath(string path, bool isFolder = false)
	{
		var builder = StringBuilderPool.Alloc();
		builder.Append(Application.persistentDataPath);
		builder.Append("/");
		builder.Append(path.ToLower());
		if (!isFolder && !path.EndsWith(".u"))
		{
			builder.Append(".u");
		}
		temp_str = builder.ToString();
		builder.Recycle();
		return temp_str;
	}

	public static string GetBundleCDNUrl(string bundleName, bool isFolder = false)
	{
		var builder = StringBuilderPool.Alloc();
		builder.Append(ChannelConfig.CurCDNUrl);
		builder.Append("/");
		builder.Append(bundleName.ToLower());
		if(!isFolder && !bundleName.EndsWith(".u"))
			builder.Append(".u");

		string result = builder.ToString();
		builder.Recycle();
		
		return result;
	}

	public static string GetBundleStreammingPath(string path, bool isFolder = false)
	{
		var builder = StringBuilderPool.Alloc();
		builder.Append(Application.streamingAssetsPath);
		builder.Append("/");
		builder.Append(path.ToLower());
		if (!isFolder && !path.EndsWith(".u"))
		{
			builder.Append(".u");
		}
		temp_str = builder.ToString();
		builder.Recycle();
		return temp_str;
	}

	/// <summary>
	/// 将以 "Assets/"开头的相对路径转换为绝对路径
	/// </summary>
	public static string GetAbsolutePath(string relativePath) => $"{Application.dataPath}/{relativePath[6..]}";

	/// <summary>
	/// 将绝对路径转换为以 "Assets/" 开头的相对路径
	/// </summary>
	public static string GetRelativePath(string absolutePath) => $"Assets{absolutePath[Application.dataPath.Length..]}";
}


