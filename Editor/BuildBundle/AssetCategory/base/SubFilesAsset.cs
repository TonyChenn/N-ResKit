/// <summary>
/// 打包文件夹下所有子文件，每个子文件单独打包
/// </summary>
public class SubFilesAsset : AssetCategory
{
	public SubFilesAsset(string srcFolder, string filter, string outputFolder) 
		: base(srcFolder, filter, outputFolder)
	{
	}

	internal override BaseBundle[] GetAssetBundles()
	{
		string[] assetPaths = GetAssets();
		BaseBundle[] items = new BaseBundle[assetPaths.Length];

		for (int i = 0, iMax = assetPaths.Length; i < iMax; i++)
		{
			items[i] = new SingleFile(assetPaths[i], OutputFolder);
		}

		return items;
	}
}
