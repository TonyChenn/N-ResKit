using NCore;

public abstract class ResLoader:NormalSingleton<ResLoader>
{
	protected abstract string PackagePath { get; set; }
	public static T Load<T>(string pkgPath, string bundleName) where T : UnityEngine.Object
	{
#if UNITY_EDITOR
		if (GameConfig.UseLocalAsset)
		{
			string path = $"{Singleton.PackagePath}/{bundleName}.u";
			T result = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
			return result;
		}
#endif
		return default(T);
	}
}
