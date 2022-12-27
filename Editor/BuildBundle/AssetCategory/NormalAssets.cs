using System.Collections;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

public class NormalAssets<T> : AssetCategory where T : UObject
{
    public NormalAssets(string srcFolder, string filter, string outputFolder) 
        : base(srcFolder, filter, outputFolder)
    {
    }

    public override BaseBundle[] GetAssetBundles()
    {
        string[] assetPaths = base.GetAssets();
        BaseBundle[] items = new BaseBundle[assetPaths.Length];

        for (int i = 0, iMax = assetPaths.Length; i < iMax; i++)
            items[i] = new SingleFile<T>(assetPaths[i], OutputFolder, SrcFolder);

        return items;
    }
}
