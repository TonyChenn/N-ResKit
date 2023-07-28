using System;
using System.Collections.Generic;
using NCore;

public class BundleLoader : ILoader
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

		res = new BundleRes(bundleName);
		res.LoadSync();
		res.AddRef();
		loadedDict[bundleName] = res;
		ResMgr.LoadedABDict[bundleName] = res;

		string[] deps = DependManifest.GetAllDependencies(bundleName);
		if (deps != null)
		{
			for (int i = 0, iMax = deps.Length; i < iMax; i++)
			{
				LoadSync(deps[i]);
			}
		}

		return res;
	}

	public void LoadAsync(string bundleName, Action<BundleRes> onLoaded)
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

		Action<BundleRes> loadWrapAction = (_res) =>
		{
			_res.AddRef();
			loadedDict[_res.bundleName] = _res;
			ResMgr.LoadedABDict[_res.bundleName] = _res;
			loadingDict.Remove(_res.bundleName);
			ResMgr.LoadingABDict[_res.bundleName] = _res;

			res.DependList.Add(_res);
			--res.LoadingCount;
			if (res.State == ResState.LoadSuccess && res.LoadingCount == 0)
			{
				onLoaded?.Invoke(res);
			}
		};
		res = new BundleRes(bundleName);
		string[] deps = DependManifest.GetAllDependencies(bundleName);
		if (deps != null && deps.Length > 0)
		{
			res.LoadingCount = deps.Length;
			for (int i = 0, iMax = deps.Length; i < iMax; i++)
			{
				LoadAsync(deps[i], loadWrapAction);
			}
		}
		res.AddLoadedEvent(() =>
		{
			res.AddRef();
			loadedDict[bundleName] = res;
			loadingDict.Remove(bundleName);
			ResMgr.LoadedABDict[bundleName] = res;
			ResMgr.LoadingABDict.Remove(bundleName);

			if (res.State == ResState.LoadSuccess && res.LoadingCount == 0)
			{
				onLoaded?.Invoke(res);
			}
		});
		res.LoadAsync();
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
	private static DefaultObjectPool<BundleLoader> loaderPool = new DefaultObjectPool<BundleLoader>((_loader) =>
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
