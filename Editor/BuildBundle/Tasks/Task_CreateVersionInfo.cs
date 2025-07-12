using NCore;
using NDebug;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 创建版本文件与资源清单文件
/// </summary>
public class Task_CreateVersionInfo
{
	private static string BuildRootFolder { get { return Path_BuildBundle.BuildBundleFolderPath; } }
	private static string VersionFile { get { return $"{BuildRootFolder}/version.data"; } }
	private static bool IsStreammingFolder { get { return Path_BuildBundle.SelectedBuildPlaceIndex == 0; } }

	/// <summary>
	/// 生成资源列表
	/// </summary>
	public static async void CreateHashFile()
	{
		StringBuilder builder = new(512);

		CalcuteFolderBundle(BuildRootFolder, ref builder);
		await Task.Delay(1000);
		byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
		string md5 = MD5Helper.ComputeHash(data);
		string saveName = IsStreammingFolder ? "res_manifest" : md5;

		string savePath = $"{BuildRootFolder}/{saveName}.csv";
		File.WriteAllText(savePath, builder.ToString());
		Log.GreenInfo($"创建资源清单完毕：" + savePath);

		CreateVersionFile(md5);
	}

	private static void CalcuteFolderBundle(string directory, ref StringBuilder builder)
	{
		string[] files = Directory.GetFiles(directory, "*.u", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			byte[] buffer = File.ReadAllBytes(files[i]);
			string hash = MD5Helper.ComputeHash(buffer);

			string filePath = files[i].Replace('\\', '/');
			filePath = filePath.Replace(directory, "").TrimStart('/');
			builder.AppendLine($"{filePath.ToLower()},{hash},{buffer.Length}");
		}
	}

	// 创建版控文件
	private static void CreateVersionFile(string md5, string appleExamVersion = "")
	{
		Version.VersionInfo data = new Version.VersionInfo();
		if (File.Exists(VersionFile))
		{
			data = JsonUtility.FromJson<Version.VersionInfo>(File.ReadAllText(VersionFile));
		}

		++data.smallVersion;
		data.appleExamVersion = appleExamVersion;
		data.md5 = md5;
		data.time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
		data.cdn1 = Path_BuildBundle.CDNUrl1;
		data.cdn2 = Path_BuildBundle.CDNUrl2;

		string json = JsonUtility.ToJson(data, false);
		File.WriteAllText(VersionFile, json);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		Log.GreenInfo($"版本文件创建完毕：{data.bigVersion}.{data.smallVersion} | {"" + data.appleExamVersion} | {md5} | {data.time}");
		if (!IsStreammingFolder)
		{
			string tmp_version = $"{BuildRootFolder}/version_{data.bigVersion}_{data.smallVersion}.data";
			File.Copy(VersionFile, tmp_version, true);
			ModifyBundleName();
		}
	}

	public static void ModifyBundleName()
	{
		string[] files = Directory.GetFiles(BuildRootFolder, "*.u", SearchOption.AllDirectories);

		foreach (var path in files)
		{
			string md5 = MD5Helper.GetFileMD5(path);
			string fileName = Path.GetFileName(path);
			string newFilePath = path.Replace(fileName, md5) + ".u";
			if (File.Exists(newFilePath)) { File.Delete(newFilePath); }
			Log.Info(path + "\t" + newFilePath);
			File.Move(path, newFilePath);
		}
	}
}

