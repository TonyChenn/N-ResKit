using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCore;
using UnityEngine;

public class BundleLoader
{
	private readonly Dictionary<string, BundleRes> loadingDict;
	private readonly Dictionary<string, BundleRes> loadedDict;
	public BundleLoader()
	{
		loadingDict = new Dictionary<string, BundleRes>(8);
		loadedDict = new Dictionary<string, BundleRes>(8);
	}

	public BundleRes LoadSync(string bundleName)
	{
		BundleRes res = GetRes(bundleName);
		if (res != null) { return res; }

		// Get deps
		var depList = new Stack<string>();
		GetAssetsWithDeps(bundleName, ref depList);
		// 开始加载
		while (depList.Count > 0)
		{
			string tmp_name = depList.Pop();
			// 已经加载过了
			BundleRes bundle = GetRes(tmp_name);
			if (bundle != null) continue;

			if (ResMgr.LoadedABDict.ContainsKey(tmp_name))
			{
				BundleRes b = ResMgr.LoadedABDict[tmp_name] as BundleRes;
				b.AddRef();
				loadedDict[tmp_name] = b;
				continue;
			}

			BundleRes tmp = new BundleRes(tmp_name);
			tmp.LoadSync();
			loadedDict[tmp_name] = tmp;
			ResMgr.LoadedABDict[tmp_name] = tmp;
			tmp.AddRef();
		}

		return GetRes(bundleName);
	}

	public async Task LoadAsync(string bundleName, Action<BundleRes> onLoaded)
	{
		// 已经加载完毕
		BundleRes res = GetRes(bundleName);
		if (res != null) { onLoaded(res); return; }

		// 加载中
		if (loadingDict.ContainsKey(bundleName))
		{
			res.AddLoadedEvent(() => { onLoaded(res); });
			return;
		}
		else if (ResMgr.LoadingABDict.ContainsKey(bundleName))
		{
			loadingDict[bundleName] = ResMgr.LoadingABDict[bundleName] as BundleRes;
			loadingDict[bundleName].AddRef();
			res.AddLoadedEvent(() => { onLoaded(res); });
			return;
		}

		// 获取依赖
		Stack<string> depList = new();
		GetAssetsWithDeps(bundleName, ref depList);

		// 异步加载
		while (depList.Count > 0)
		{
			var dep = depList.Pop();
			BundleRes bundle = new BundleRes(dep);
			bundle.AddLoadedEvent(() =>
			{

			});
			bundle.LoadAsync();
		}
	}

	


	/// <summary>
	/// 获取资源与所有依赖资源列表
	/// </summary>
	/// --------------------------------------------
	/// 存储顺序如下：
	/// A   A1 (A11 A12 A13)   A2(A21 A22 (A221 A222 a223))
	/// 加载时:
	/// 需要从后向前加载，先加载依赖再加载本包
	/// --------------------------------------------
	/// <param name="bundleName"></param>
	/// <param name="list"></param>
	private void GetAssetsWithDeps(string bundleName, ref Stack<string> stack)
	{
		stack.Push(bundleName);
		string[] deps = DependManifest.GetAllDependencies(bundleName);
		foreach (string dep in deps) { GetAssetsWithDeps(dep, ref stack); }
	}
	private BundleRes GetRes(string bundleName)
	{
		if (loadedDict.ContainsKey(bundleName))
		{
			return loadedDict[bundleName];
		}
		else if (ResMgr.LoadedABDict.ContainsKey(bundleName))
		{
			loadedDict[bundleName] = ResMgr.LoadedABDict[bundleName] as BundleRes;
			loadedDict[bundleName].AddRef();
			return loadedDict[bundleName];
		}
		return null;
	}

	#region LoaderPool
	private static readonly DefaultObjectPool<BundleLoader> loaderPool = new((_loader) =>
	{
		foreach (var item in _loader.loadingDict) item.Value.ClearLoadedEvent();
		foreach (var item in _loader.loadedDict) item.Value.RemoveRef();

		_loader.loadingDict.Clear();
		_loader.loadedDict.Clear();
	});
	public static BundleLoader Alloc() => loaderPool.Alloc();
	public void Recycle()
	{
		loaderPool.Recycle(this);
	}
	#endregion
}
