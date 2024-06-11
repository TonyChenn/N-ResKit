using NCore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BundleRes : BaseRes
{
	public BundleRes(string bundleName)
	{
		this.BundleName = bundleName;
	}

	public AssetBundle Bundle { get => Asset as AssetBundle; }


	///TODO 这里需要重写
	public override bool IsExist()
	{
		return true;
		//string bundlePath = PathUtil.GetBundlePath(bundleName);
		//return System.IO.File.Exists(bundlePath);
	}


	public override void LoadSync()
	{
		State = ResState.Loading;
		string path = PathUtil.GetBundlePath(BundleName);
		Asset = AssetBundle.LoadFromFile(path);
		State = ResState.LoadSuccess;
	}

	public override void LoadAsync()
	{
		State = ResState.Loading;
		string path = PathUtil.GetBundlePath(BundleName);

		var request = AssetBundle.LoadFromFileAsync(path);
		request.completed += (operation) =>
		{
			Asset = request.assetBundle;
			State = ResState.LoadSuccess;
			onLoadedEvent?.Invoke();
		};
	}

	protected override void OnZeroRef()
	{
		if (Bundle != null) Bundle.Unload(true);
		Asset = null;
	}
}
