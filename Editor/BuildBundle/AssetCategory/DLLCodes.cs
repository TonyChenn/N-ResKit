public class DLLCodes : SingleFolderAsset
{
	public DLLCodes(string srcFolder, string outputFolder) 
		: base(srcFolder, "f:*.bytes", outputFolder)
	{
	}

	public override void PrepareBuild()
	{
		base.PrepareBuild();
		Task_CompileDLL.CompileAndCopyDll();
	}
}
