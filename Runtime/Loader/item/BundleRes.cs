using NCore;
using System.Threading.Tasks;
using UnityEngine;

public class BundleRes : BaseRes
{
	public AssetBundleCreateRequest BundleCreateRequest{ get; private set; }

	public BundleRes(string bundleName)
	{
		this.BundleName = bundleName;
		State = ResState.Waiting;
	}

	public AssetBundle Bundle { get => Asset as AssetBundle; }


	///TODO 这里需要重写
	public override bool IsExist()
	{
		return true;
		//string bundlePath = PathUtil.GetBundlePath(bundleName);
		//return System.IO.File.Exists(bundlePath);
	}


	/// <summary>
	/// 同步加载资源(不用管依赖)
	/// </summary>
	public override void LoadSync()
	{
		State = ResState.Loading;
		string path = PathUtil.GetBundlePath(BundleName);
		Asset = AssetBundle.LoadFromFile(path);
		State = ResState.LoadSuccess;
	}

	/// <summary>
	/// 异步加载资源(不用管依赖)
	/// </summary>
	public override async Task LoadAsync()
	{
		if(State == ResState.Loading && BundleCreateRequest != null)
		{
			await BundleCreateRequest;
			return;
		}

		State = ResState.Loading;
		string path = PathUtil.GetBundlePath(BundleName);

		BundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
		AssetBundle bundle = await BundleCreateRequest;
		
		// 异步加载过程中被取消了
		if(State == ResState.Cancel) return;

		Asset = bundle;
		State = ResState.LoadSuccess;

		BundleCreateRequest = null;
	}

	public async Task<BundleRes> WaitAsync()
	{
		if (BundleCreateRequest != null)
		{
			await BundleCreateRequest;
			return this;
		}
		return null;
	}

	protected override void OnZeroRef()
	{
		if (Bundle != null) Bundle.Unload(true);
		Asset = null;
	}
}
