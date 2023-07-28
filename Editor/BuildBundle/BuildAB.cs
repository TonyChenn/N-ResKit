using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

public class BuildAB
{
	private static string VersionFile => $"{BuildRootFolder}/version.data";

	private static string m_targetFolder;
	private static string BuildRootFolder
	{
		get { return m_targetFolder; }
		set { m_targetFolder = value.Replace("\\", "/").TrimEnd('/'); }
	}

	public static IEnumerator Build(List<AssetCategory> categories, string targetFolder)
	{
		// init
		BuildRootFolder = targetFolder;

		if (categories == null || categories.Count == 0)
		{
			EditorUtility.DisplayDialog("错误", "没有要打包的配置信息", "好的");
			yield break;
		}

		// 计算Hash之前
		categories.ForEach((asset) => { asset.OnBeforeComputeHash(); });


		if (true)
		{
			yield return "正在打包...";
			buildHandler(BuildRootFolder, categories);
			yield return "打包完成";

			// 打包完成事件
			categories.ForEach((asset) => { asset.OnBuildFinished(); });
		}
		// 打包结束事件
		categories.ForEach((asset) => { asset.OnAllBuildCompleted(); });

		// dispose
		categories.ForEach((asset) => { asset.Dispose(); });
	}

	private static void buildHandler(string folder, List<AssetCategory> categories)
	{
		// 清理旧资源
		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		string[] bundles = Directory.GetFiles(folder, "*.u", SearchOption.AllDirectories);
		foreach (string bundle in bundles) { File.Delete(bundle); }
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		// 准备打包
		List<AssetBundleBuild> list = new List<AssetBundleBuild>(128);
		foreach (var item in categories)
		{
			// 无需打包资源
			if (item is UnBuildAssets)
			{
				(item as UnBuildAssets).Build(folder);
			}
			else
			{
				item.PrepareBuild();
				list.AddRange(item.AssetBundleBuilds);
			}
		}

		// build
		BuildPipeline.BuildAssetBundles(folder, list.ToArray(), Path_BuildBundle.BundleCompression, EditorUserBuildSettings.activeBuildTarget);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		// 整理AssetBundle清单文件
		string[] manifests = Directory.GetFiles(folder, "*.manifest", SearchOption.AllDirectories);
		foreach (var item in manifests) { File.Delete(item); }

		string fileName = Path.GetFileNameWithoutExtension(folder);
		if (File.Exists($"{folder}/{fileName}"))
		{
			File.Move($"{folder}/{fileName}", $"{folder}/bundle_manifest.u");
		}

		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
	}

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
			using (FileStream fs = new FileStream(VersionFile, FileMode.Open))
			{
				BinaryReader br = new BinaryReader(fs);
				smallVersion = br.ReadInt32();
				bigVersion = br.ReadInt32();
				appleExamVersion = br.ReadString();
			}
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
