using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Task_BuildAB
{
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

		// 打包前事件
		categories.ForEach(asset => { asset.PrepareBuild(); });

		// 开始打包
		yield return "正在打包...";
		buildHandler(BuildRootFolder, categories);
		yield return "打包完成";

		// 打包完成事件
		categories.ForEach((asset) => { asset.OnBuildFinished(); });
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
		List<AssetBundleBuild> list = new(128);
		foreach (var item in categories)
		{
			// 无需打包资源
			if (item is CustomBuildAsset temp)
			{
				temp.Build(folder);
			}
			else
			{
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
}
