using UnityEditor;
using UObject = UnityEngine.Object;

public class DLLCodes : AssetCategory
{
    public DLLCodes(string srcFolder, string outputFolder) :
        base(srcFolder, "f:*.dll.bytes", outputFolder)
    {
    }

    public override void OnBeforeComputeHash()
    {
        BuildDll.CompileDll(EditorUserBuildSettings.activeBuildTarget);
    }

    public override BaseBundle[] GetAssetBundles()
    {
        string[] dllpaths = base.GetAssets();
        BaseBundle[] items = new BaseBundle[dllpaths.Length];
        for (int i = 0,iMax = dllpaths.Length; i < iMax; i++)
        {
            items[i] = new SingleFile<UObject>(dllpaths[i], OutputFolder, SrcFolder);
        }

        return items;
    }
}
