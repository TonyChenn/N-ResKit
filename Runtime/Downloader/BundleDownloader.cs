using NCore;
using NDebug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using UnityEngine;

/// <summary>
/// 资源状态
/// </summary>
public enum BundleState
{
	None,
	Added,              // 增
	Removed,            // 删
	Modified,           // 改
	NoChange,           // 不变
	DownloadWhenUse,    // 使用时再下载
}

/// <summary>
/// 下载状态
/// </summary>
public enum DownloadState
{
	Wait,
	Downloading,
	Downloaded,
	DownloadError,
}

public class BundleDownloader : MonoSinglton<BundleDownloader>
{
	public const int MaxReTryCount = 3;
	public const int MaxDownloadThreadCount = 10;
	public const int TimeOut = 10 * 1000;

	private DateTime startDownloadTime;
	private List<int> downloadingThreads = new List<int>(MaxDownloadThreadCount);
	private int maxThreadID = 0;
	private Dictionary<string, int> downloadErrorDict = new Dictionary<string, int>();

	private static Queue<DownloadItem> needDownloadQuque = null;

	// 下载总字节数
	private long totalNeedDownloadBytes = 0;
	// 下载完成总字节数
	private long downloadedBytes = 0;
	// 当前正在下载的字节数
	private Dictionary<int, long> downloadingBytes = new Dictionary<int, long>();
	private BundleDownloader() { }

	private Action downloadFinishCallback = null;
	private Action<int> refreshUIHandler = null;

	public void DownloadAsync(List<ResManifest.ResUnit> downloadList, Action<int> refreshUIHandler, Action callback = null)
	{
		if (downloadList != null && downloadList.Count > 0)
		{
			needDownloadQuque ??= new Queue<DownloadItem>(downloadList.Count);
			foreach (var unit in downloadList)
			{
				int size = int.Parse(unit.size);
				totalNeedDownloadBytes += size;

				var item = new DownloadItem(unit.bundleName, unit.md5, size);
				needDownloadQuque.Enqueue(item);
			}
		}
		if (needDownloadQuque == null) return;
		if (needDownloadQuque.Count == 0) return;

		this.refreshUIHandler = refreshUIHandler;
		downloadFinishCallback += callback;

		int count = MaxDownloadThreadCount - downloadingThreads.Count;

		for (int i = 0; i < count; i++)
		{
			++maxThreadID;

			if (!downloadingThreads.Contains(maxThreadID))
			{
				downloadingThreads.Add(maxThreadID);
			}

			downloadHandler(maxThreadID);
		}
		return;
	}


	private async void downloadHandler(int threadID)
	{
		byte[] buffer = new byte[102400];

		downloadingBytes[threadID] = 0;
		while (needDownloadQuque.Count > 0)
		{
			DownloadItem item = needDownloadQuque.Dequeue();
			item.DownloadState = DownloadState.Downloading;

			string tmpPath = item.TempPath;
			FileInfo fileInfo = new FileInfo(tmpPath);
			long existingSize = fileInfo.Exists ? fileInfo.Length : 0;

			var request = new HttpRequestMessage(HttpMethod.Get, item.DownloadUrl);
			if (existingSize > 0)
			{
				request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingSize, null);
			}

			var httpClient = new HttpClient();
			var response = await httpClient.SendAsync(request);

			// 下载完成后检查
			Action fileDownloadedCheck = () =>
			{
				if (checkFileDownloadSuccess(item))
				{
					File.Move(item.TempPath, item.SavePath);
					downloadedBytes += item.Size;
					item.DownloadState = DownloadState.Downloaded;
					return;
				}
				downloadErrorHandler(threadID, item);
			};

			try
			{
				if (response.StatusCode == System.Net.HttpStatusCode.RequestedRangeNotSatisfiable)
				{
					fileDownloadedCheck();
					continue;
				}
				var dir = Path.GetDirectoryName(item.TempPath);
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				var totalSize = existingSize + response.Content.Headers.ContentLength.GetValueOrDefault();
				using FileStream fileStream = new FileStream(item.TempPath, FileMode.Append, FileAccess.Write, FileShare.Write);
				var stream = await response.Content.ReadAsStreamAsync();


				int readLength = 0;
				int length = 0;
				while ((length = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					readLength += length;
					Log.Info($"thread {threadID} downloading:{item.Name}--->{readLength}/{totalSize}");
					await fileStream.WriteAsync(buffer, 0, length);
					downloadingBytes[threadID] = readLength + existingSize;

					long size = 0;
					downloadingBytes.Values.ToList().ForEach(v => size += v);
					float percent = size * 99 / totalNeedDownloadBytes;
					refreshUIHandler?.Invoke((int)percent);
				}
				await fileStream.FlushAsync();
				await fileStream.DisposeAsync();
				await stream.DisposeAsync();

				fileDownloadedCheck();
			}
			catch (Exception ex)
			{
				downloadErrorHandler(threadID, item);
				Log.RedInfo(ex.Message);
			}
		}
		Debug.Log($"download thread: {threadID} is finished");
		downloadingThreads.Remove(threadID);

		if (downloadingThreads.Count == 0)
		{
			downloadFinishCallback?.Invoke();
			downloadFinishCallback = null;
			maxThreadID = 0;
			Dispose();
		}
	}

	/// <summary>
	/// 检查下载文件是否成功
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
	private bool checkFileDownloadSuccess(DownloadItem item)
	{
		string tmpPath = item.TempPath;
		if (!File.Exists(tmpPath)) return false;

		string realMd5 = MD5Helper.GetFileMD5(tmpPath);
		if (item.MD5 != null && realMd5 != item.MD5)
		{
			Log.RedInfo($"md5 not match: file:{realMd5}\t record: {item.MD5}");
			return false;
		}
		return true;
	}

	/// <summary>
	/// 下载失败处理
	/// </summary>
	/// <param name="errorItem"></param>
	private void downloadErrorHandler(int threadID, DownloadItem errorItem)
	{
		if (errorItem == null) return;
		errorItem.DownloadState = DownloadState.DownloadError;

		downloadingBytes[threadID] = 0;
		// 下载出错，重新加入队列
		needDownloadQuque.Enqueue(errorItem);
		if (File.Exists(errorItem.TempPath)) File.Delete(errorItem.TempPath);

		if (!downloadErrorDict.ContainsKey(errorItem.Name))
		{
			downloadErrorDict[errorItem.Name] = 1;
		}
		else
		{
			++downloadErrorDict[errorItem.Name];
			if (downloadErrorDict[errorItem.Name] >= MaxReTryCount)
			{
				// 多次下载失败，要求检查网络环境。
				Application.Quit();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
			}
		}
		Log.RedInfo($"下载出错\t{downloadErrorDict[errorItem.Name]} 次：{errorItem.Name}\t{errorItem.DownloadUrl}");
	}
	
	private Vector2 scrollPos = Vector2.zero;
	private void OnGUI()
	{
#if UNITY_EDITOR
		GUILayout.BeginArea(new Rect(0, Screen.height / 2, Screen.width/2, Screen.height/2));
		GUILayout.Label("下载中");
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width/2), GUILayout.Height(Screen.height / 2));
		foreach (var item in needDownloadQuque)
		{
			GUILayout.Label($"{item.Name}\t{item.DownloadState.ToString()}");
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
#endif
	}
}
