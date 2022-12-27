using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnBuildAssets : AssetCategory
{
    public UnBuildAssets(string srcFolder, string filter, string outputFolder) 
        : base(srcFolder, filter, outputFolder)
    {
    }

    public override BaseBundle[] GetAssetBundles()
    {
        string[] assets = GetAssets();
        var items = new BaseBundle[assets.Length];
        for (int i = 0,iMax = assets.Length; i < iMax; i++)
        {
            items[i] = new UnBuildAsset(assets[i], OutputFolder, SrcFolder);
        }

        return items;
    }

    public void Build(string folder)
    {
        for (int i = 0,iMax = Bundles.Length; i < iMax; i++)
        {
            ((UnBuildAsset)Bundles[i]).Build(folder);
        }
    }
}
