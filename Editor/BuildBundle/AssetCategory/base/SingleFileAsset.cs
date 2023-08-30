/// <summary>
/// 单个文件打包AssetBundle
/// </summary>
public class SingleFileAsset : AssetCategory
{
	public SingleFileAsset(string filePath, string outputFolder)
		: base(filePath, null, outputFolder)
	{
	}

	internal override BaseBundle[] GetAssetBundles()
	{
		return new BaseBundle[] { new SingleFile(SrcFolder, OutputFolder) };
	}
}

internal class SingleFile : BaseBundle
{
	protected string m_assetPath;
	string m_Name;


	public string AssetPath { get { return m_assetPath; } }

	public override string Name => m_Name;

	public override string[] AssetNames => new string[] { m_assetPath };

	public SingleFile(string assetPath, string outputFolder)
		: base(outputFolder)
	{
		m_assetPath = assetPath.Replace("\\", "/").TrimEnd('/');
		m_Name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
	}
}


