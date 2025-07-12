using System.Threading.Tasks;
using UnityEngine;
using UObject = UnityEngine.Object;

public abstract class BaseRes : RefCounter
{
	public string BundleName { get; protected set; }
	protected UObject Asset;
	protected ResState state;

	public int LoadingCount { get; set; }

	public ResState State
	{
		get { return state; }
		set { state = value; }
	}

	public abstract bool IsExist();
	public abstract void LoadSync();
	public abstract Task LoadAsync();
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
