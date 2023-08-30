public class NonBuildAsset : CustomBuildAsset
{
	public NonBuildAsset(string srcFolder, string filter, string outputFolder)
		: base(srcFolder, filter, outputFolder)
	{
	}

	public override void Build(string buildRootFolder)
	{
		string[] items = GetAssets();
		foreach (string item in items)
		{
			string fileName = System.IO.Path.GetFileName(item);
			string target = $"{buildRootFolder}/{OutputFolder.ToLower()}/{fileName.ToLower()}";
			System.IO.File.Copy(item, target, true);
		}
	}

	internal override BaseBundle[] GetAssetBundles() { return null; }
}
