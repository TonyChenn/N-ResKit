using System;
using System.IO;
using System.Net;
using UnityEngine;

public class DownloadItem
{
	public string Name { get; private set; }
	public string MD5 { get; private set; }
	public int Size { get; private set; }

	/// <summary>
	/// 远程地址
	/// </summary>
	public string DownloadUrl { get; private set; }
	/// <summary>
	/// 真实保存路径
	/// </summary>
	public string SavePath { get; private set; }
	/// <summary>
	/// 下载临时文件路径
	/// </summary>
	public string TempPath { get; private set; }

	public BundleState State = BundleState.None;
	public DownloadState DownloadState = DownloadState.Wait;

	public Action DownloadCompleteCallback;


	public DownloadItem(string name, string md5, int size)
	{
		Name = name;
		MD5 = md5;
		Size = size;
		State = BundleState.None;

		SavePath = PathUtil.GetBundlePersistPath(name);
		TempPath = SavePath + ".tmp";
		string md5Name = ResManifest.GetBundleMD5Name(name);
		DownloadUrl = $"{ChannelConfig.CurChannelConfig.CurCDNUrl}/{md5Name}";
		DownloadState = DownloadState.Wait;
	}

	public bool NeedDownload { get { return State == BundleState.Added || State == BundleState.Modified; } }
}
