public class SubFoldersAsset : AssetCategory
{
	public SubFoldersAsset(string srcFolder, string filter, string outputFolder) 
		: base(srcFolder, filter, outputFolder)
	{

	}

	internal override BaseBundle[] GetAssetBundles()
	{
		string[] subfolders = System.IO.Directory.GetDirectories(SrcFolder);
		var result = new SingleFolder[subfolders.Length];
		for (int i = 0; i < subfolders.Length; i++)
		{
			string[] items = GetAssets(subfolders[i], Filter);
			result[i] = new SingleFolder(subfolders[i], items, OutputFolder);
		}
		return result;
	}
}
