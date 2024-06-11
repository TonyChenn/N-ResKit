using System.Collections.Generic;

public class ResMgr
{
	// 同时最大加载数量
	private const int MAX_LOAD_COUNT = 10;

	#region ChannelItemConfig
	#endregion

	private static Dictionary<string, BaseRes> loadingABDict;
	private static Dictionary<string, BaseRes> loadedABDict;
	private static Dictionary<string, BaseRes> unloadABDict;

	public static Dictionary<string, BaseRes> LoadingABDict
	{
		get
		{
			if (loadingABDict == null)
				loadingABDict = new Dictionary<string, BaseRes>();

			return loadingABDict;
		}
	}
	public static Dictionary<string, BaseRes> LoadedABDict
	{
		get
		{
			if (loadedABDict == null)
				loadedABDict = new Dictionary<string, BaseRes>();
			return loadedABDict;
		}
	}
	public static Dictionary<string, BaseRes> UnloadABDict
	{
		get
		{
			if (unloadABDict == null)
				unloadABDict = new Dictionary<string, BaseRes>();
			return unloadABDict;
		}
	}

	public static void Restart()
	{
		loadingABDict?.Clear();
		loadedABDict?.Clear();
		unloadABDict?.Clear();
	}
}
