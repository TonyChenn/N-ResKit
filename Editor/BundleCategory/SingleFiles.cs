using BuildBundle.Editor;
using UObject = UnityEngine.Object;

public class SingleFiles<T> : AssetCategory where T : UObject
{
    public SingleFiles(string srcFolder, string filter, string outputFolder) : base(srcFolder, filter, outputFolder)
    {
    }

    protected override Bundle[] GetAssetBundleItems()
    {
        string[] assets=base.GetAssets();
        Bundle[] bundles = new Bundle[assets.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            bundles[i] = new SingleFile<T>(assets[i],base.OutputFolder,base.SrcFolder);
        }

        return bundles;
    }
}
