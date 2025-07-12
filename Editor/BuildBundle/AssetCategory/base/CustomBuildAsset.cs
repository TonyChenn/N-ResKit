/// <summary>
/// 自定义构建资源
/// </summary>
public abstract class CustomBuildAsset : AssetCategory
{
	internal CustomBuildAsset(string srcFolder, string filter, string outputFolder) 
		: base(srcFolder, filter, outputFolder)
	{
	}

	public abstract void Build(string buildRootFolder);
}
