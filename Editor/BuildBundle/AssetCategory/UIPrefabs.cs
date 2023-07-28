using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIPrefabs : AssetCategory
{
    public UIPrefabs(string srcFolder, string outputFolder) 
        : base(srcFolder, "t:Prefab", outputFolder)
    {
    }

    public override BaseBundle[] GetAssetBundles()
    {
        string[] subfolders = Directory.GetDirectories(SrcFolder);
        var items = new UIPrefab[subfolders.Length];

        for (int i = 0; i < subfolders.Length; i++)
        {
            string folder = subfolders[i];
            string[] assets = GetAssets(folder, Filter);
            items[i] = new UIPrefab(SrcFolder, folder, assets, OutputFolder);
        }

        return items;
    }
}
