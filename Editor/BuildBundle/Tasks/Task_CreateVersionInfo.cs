using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 创建版本文件与资源清单文件
/// </summary>
public class Task_CreateVersionInfo
{
	private static readonly string BuildRootFolder = Path_BuildBundle.BuildBundleFolderPath;
	private static readonly string VersionFile = $"{BuildRootFolder}/version.data";

	/// <summary>
	/// 生成资源列表
	/// </summary>
	public static async void CreateHashFile()
	{
		bool streammingFolder = BuildRootFolder == Application.streamingAssetsPath;

		StringBuilder builder = new(512);

		CalcuteFolderBundle(new DirectoryInfo(BuildRootFolder), ref builder);
		await Task.Delay(1000);
		byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
		string md5 = MD5Helper.ComputeHash(data);
		string saveName = streammingFolder ? "res_manifest" : md5;
		File.WriteAllText($"{BuildRootFolder}/{saveName}.csv", builder.ToString());

		CreateVersionFile(md5, streammingFolder);
	}

	private static void CalcuteFolderBundle(DirectoryInfo directory, ref StringBuilder builder)
	{
		foreach (var dic in directory.GetDirectories())
		{
			CalcuteFolderBundle(dic, ref builder);
		}

		foreach (FileInfo file in directory.GetFiles("*.u"))
		{
			byte[] buffer = File.ReadAllBytes(file.FullName);
			string hash = MD5Helper.ComputeHash(buffer);
			string filePath = file.FullName;
			filePath = filePath.Replace('\\', '/');
			filePath = filePath.Replace(BuildRootFolder, "").TrimStart('/');
			builder.AppendLine($"{filePath.ToLower()},{hash},{buffer.Length}");
		}
	}

	// 创建版控文件
	private static void CreateVersionFile(string md5, bool streammingFolder)
	{
		Int32 smallVersion = 0;
		Int32 bigVersion = 0;
		string appleExamVersion = "1.0";
		long time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


		if (File.Exists(VersionFile))
		{
			using FileStream fs = new(VersionFile, FileMode.Open);
			BinaryReader br = new(fs);
			smallVersion = br.ReadInt32();
			bigVersion = br.ReadInt32();
			appleExamVersion = br.ReadString();
		}
		++smallVersion;

		using (BinaryWriter writer = new BinaryWriter(File.Open(VersionFile, FileMode.OpenOrCreate)))
		{
			writer.Write(smallVersion);
			writer.Write(bigVersion);
			writer.Write(appleExamVersion);
			writer.Write(md5);
			writer.Write(time);
		}
		if (!streammingFolder)
		{
			string tmp_version = $"{BuildRootFolder}/version_{bigVersion}_{smallVersion}.data";
			File.Copy(VersionFile, tmp_version, true);
			ModifyBundleName(BuildRootFolder);
		}
	}

	public static void ModifyBundleName(string folder)
	{
		string[] files = Directory.GetFiles(folder, "*.u", SearchOption.AllDirectories);

		foreach (var path in files)
		{
			string md5 = MD5Helper.GetFileMD5(path);
			string newFilePath = path.Replace(Path.GetFileNameWithoutExtension(path), md5);
			if (File.Exists(newFilePath)) { File.Delete(newFilePath); }
			File.Move(path, newFilePath);
		}
	}
}
