using NCore;
using UnityEngine;


public static class PathUtil
{
	private static string temp_str = string.Empty;

	public static string GetBundleFullPath(string bundleName, bool isFolder = false)
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
	#region 

	private static string GetBundleStreammingPath(string path, bool isFolder = false)
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
	#endregion
}


