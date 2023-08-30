/// <summary>
/// 打包当前文件夹
/// </summary>
public class SingleFolderAsset : AssetCategory
{
	public SingleFolderAsset(string srcFolder, string filter, string outputFolder) 
		: base(srcFolder, filter, outputFolder)
	{
	}

	internal override BaseBundle[] GetAssetBundles()
	{
		string[] assets = GetAssets(SrcFolder, Filter);
		var folder = new SingleFolder(SrcFolder, assets, OutputFolder);
		return new BaseBundle[] {folder };
	}
}


internal class SingleFolder : BaseBundle
{
	private string m_name;
	private string[] m_assetPaths;

	public SingleFolder(string folderPath, string[] assetPath, string outputFolder) : base(outputFolder)
	{
		m_assetPaths = assetPath;
		m_name = System.IO.Path.GetFileNameWithoutExtension(folderPath);
	}

	public override string Name => m_name;
	public override string[] AssetNames => m_assetPaths;
}
