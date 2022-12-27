using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIPrefab : BaseBundle
{
    private string m_SrcFolder;
    private string[] m_AssetPaths;
    private string m_Name;

    public UIPrefab(string srcFolder,string subFolder,string[] assetPaths, string outputFolder)
        : base(outputFolder)
    {
        m_SrcFolder = srcFolder;
        m_AssetPaths = assetPaths;
        m_Name = Path.GetFileNameWithoutExtension(subFolder);
    }

    public override string Name => throw new System.NotImplementedException();

    public override string[] AssetNames => throw new System.NotImplementedException();

    protected override string ComputeHash()
    {
        throw new System.NotImplementedException();
    }
}
