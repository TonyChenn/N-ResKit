public class DLLCodes : SubFoldersAsset
{
	public DLLCodes(string srcFolder, string outputFolder) 
		: base(srcFolder, "f:*.bytes", outputFolder)
	{
	}
}
