using System.IO;
using UnityEngine.WSA;

public class UnBuildAsset : SingleFile<UnityEngine.Object>
{
    private new string m_assetPath;
    private string m_outputFolder;

    public UnBuildAsset(string assetPath, string outputFolder, string srcFolder) 
        : base(assetPath, outputFolder, srcFolder)
    {
        m_assetPath = assetPath;
        m_outputFolder = outputFolder;
    }

    public void Build(string folder)
    {
        string name = Path.GetFileName(m_assetPath);
        string targetPath = string.Format($"{folder}/{m_outputFolder.ToLower()}/{name.ToLower()}");
        File.Delete(targetPath);
        File.Copy(m_assetPath, targetPath);
    }

    public override string Ext => Path.GetExtension(m_assetPath);

    public override string[] AssetNames => null;
}
