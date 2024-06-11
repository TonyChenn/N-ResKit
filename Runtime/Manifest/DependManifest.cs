using NCore;
using NDebug;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 打包AssetBundle后生成的依赖清单
/// </summary>
public class DependManifest : NormalSingleton<DependManifest>
{
	// Bundle的依赖词典
	private Dictionary<string, string[]> dependManifestDict;


	private DependManifest()
	{
		dependManifestDict = new Dictionary<string, string[]>(32);
		LoadManifest();
	}

	#region API
	public static void Restart()
	{
		AssetBundle.UnloadAllAssetBundles(true);
		Singleton.dependManifestDict.Clear();
		Singleton.LoadManifest();
	}

	/// <summary>
	/// 获取AssetBundle的所有依赖文件
	/// </summary>
	/// <param name="bundleName">无需加 ".u"</param>
	/// <returns></returns>
	public static string[] GetAllDependencies(string bundleName)
	{
		if (bundleName.EndsWith(".u"))
			bundleName = bundleName.Substring(0, bundleName.LastIndexOf("."));

		if (Singleton.dependManifestDict.ContainsKey(bundleName))
			return Singleton.dependManifestDict[bundleName];

		return null;
	}
	#endregion


	/// <summary>
	/// AB包的依赖清单
	/// </summary>
	private void LoadManifest()
	{
		var path = PathUtil.GetBundlePath("bundle_manifest");
		dependManifestDict.Clear();

		var bundle = AssetBundle.LoadFromFile(path);
		var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] bundles = manifest.GetAllAssetBundles();
		for (int i = 0, iMax = bundles.Length; i < iMax; i++)
		{
			string bundleName = bundles[i].Replace(".u", "");
			string[] deps = manifest.GetAllDependencies(bundles[i]);
			for (int j = 0, jMax = deps.Length; j < jMax; j++)
				deps[j] = deps[j].Replace(".u", "");

			dependManifestDict[bundleName] = deps;
		}
		bundle.Unload(true);
		Log.GreenInfo($"ResMgr load bundle_manifest");
	}
}
