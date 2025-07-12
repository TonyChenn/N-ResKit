using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCore;
using UnityEngine;

public class BundleLoader
{
	private readonly Dictionary<string, BundleRes> loadingDict;
	private readonly Dictionary<string, BundleRes> loadedDict;
	public BundleLoader()
	{
		loadingDict ??= new Dictionary<string, BundleRes>(8);
		loadedDict ??= new Dictionary<string, BundleRes>(8);
	}

	public BundleRes LoadSync(string bundleName)
	{
		// 已经加载过了
		BundleRes res = GetRes(bundleName);
		if (res != null) { return res; }

		// 没有加载过
		// Get deps
		var depList = new Stack<string>();
		GetAssetsWithDeps(bundleName, ref depList);
		// 开始加载
		while (depList.Count > 0)
		{
			string depName = depList.Pop();
			// 已经加载过了
			BundleRes bundle = GetRes(depName);
			if (bundle != null) continue;

			// 当前loader正在加载
			if (loadingDict.ContainsKey(depName))
			{
				Debug.LogWarning("请避免使用同步加载 正在异步加载中 的资源！！！");
				loadingDict[depName].BundleCreateRequest.GetAwaiter().GetResult();
				continue;
			}
			// ResMgr正在加载
			if(ResMgr.LoadingABDict.ContainsKey(depName))
			{
				Debug.LogWarning("请避免使用同步加载 正在异步加载中 的资源！！！");
				BundleRes bundle1 = ResMgr.LoadingABDict[depName] as BundleRes;
				loadingDict[depName] = res;

				bundle1.BundleCreateRequest.GetAwaiter().GetResult();
				res.AddRef();
				continue;
			}

			BundleRes tmp = new BundleRes(depName);
			tmp.LoadSync();
			loadedDict[depName] = tmp;
			ResMgr.LoadedABDict[depName] = tmp;
			tmp.AddRef();
		}

		return GetRes(bundleName);
	}

	public async Task<BundleRes> LoadAsync(string bundleName)
	{
		// 已经加载完毕
		BundleRes res1 = GetRes(bundleName);
		if (res1 != null) { return res1; }

		// 正在加载中
		BundleRes res2 = await GetResAsync(bundleName);
		if (res2 != null) { return res1; }


		// 获取依赖
		Stack<string> depList = new();
		GetAssetsWithDeps(bundleName, ref depList);

		// 异步加载
		while (depList.Count > 0)
		{
			var depName = depList.Pop();
			// 已经加载过了
			BundleRes bundle1 = GetRes(depName);
			if (bundle1 != null) continue;

			// 当前loader正在异步加载
			if (loadingDict.ContainsKey(depName))
			{
				await loadingDict[depName].WaitAsync();
				continue;
			}
			// ResMgr正在异步加载
			if(ResMgr.LoadingABDict.ContainsKey(depName))
			{
				BundleRes res = ResMgr.LoadingABDict[depName] as BundleRes;
				loadingDict[depName] = res;

				res.AddRef();
				await res.WaitAsync();
				continue;
			}

			// 开始异步加载
			BundleRes bundle2 = new BundleRes(depName);
			ResMgr.LoadingABDict[depName] = bundle2;
			loadingDict[depName] = bundle2;
			
			await bundle2.LoadAsync();

			if (bundle2.State == ResState.Cancel)
			{
				if (bundle2.RefCount == 0)
				{
				}
			}

			loadedDict[depName] = bundle2;
			ResMgr.LoadedABDict[depName] = bundle2;
			loadingDict[depName] = null;
			ResMgr.LoadingABDict[depName] = null;
		}
		return GetRes(bundleName);
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
		if (deps == null || deps.Length == 0) return;

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

	private async Task<BundleRes> GetResAsync(string bundleName)
	{
		if (loadingDict.ContainsKey(bundleName) && loadingDict[bundleName].State == ResState.Loading)
		{
			BundleRes result = await loadingDict[bundleName].WaitAsync();
			return result;
		}
		else if (ResMgr.LoadingABDict.ContainsKey(bundleName)
				&& ResMgr.LoadingABDict[bundleName].State == ResState.Loading)
		{
			BundleRes result = await loadingDict[bundleName].WaitAsync();
			loadingDict[bundleName].AddRef();

			return result;
		}
		return null;
	}

	#region LoaderPool
	private static readonly DefaultObjectPool<BundleLoader> loaderPool = new((_loader) =>
	{
		foreach (var item in _loader.loadingDict) item.Value.RemoveRef();
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
