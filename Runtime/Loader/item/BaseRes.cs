using System;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

public abstract class BaseRes : RefCounter
{
	public string BundleName { get; protected set; }
	protected UObject Asset;
	protected ResState state;
	protected Action onLoadedEvent;

	public int LoadingCount { get; set; }

	public ResState State
	{
		get { return state; }
		set { state = value; }
	}

	/// <summary>
	/// 添加加载完成回调事件
	/// </summary>
	public void AddLoadedEvent(Action onLoaded)
	{
		if (onLoaded == null) return;

		onLoadedEvent += onLoaded;
	}

	/// <summary>
	/// 移除加载完成回调事件
	/// </summary>
	public void RemoveLoadedEvent(Action onLoaded)
	{
		if (onLoaded == null) return;

		onLoadedEvent -= onLoaded;
	}

	public void ClearLoadedEvent()
	{
		onLoadedEvent = null;
	}

	public abstract bool IsExist();
	public abstract void LoadSync();
	public abstract void LoadAsync();
}

public enum ResState
{
	Waiting,
	Loading,
	Downloading,
	LoadSuccess,
	LoadFail,
	Cancel,
	Dispose
}
