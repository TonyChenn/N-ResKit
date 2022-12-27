using System.IO;

public class SingleFolder : BaseBundle
{
    private string m_name;
    private string[] m_assetPaths;

    public SingleFolder(string folderPath, string[] assetPath, string outputFolder) : base(outputFolder)
    {
        m_assetPaths = assetPath;
        m_name = Path.GetFileNameWithoutExtension(folderPath);
    }

    public override string Name => m_name;
    public override string[] AssetNames => m_assetPaths;

    protected override string ComputeHash()
    {
        return MD5Helper.ComputeHashWithDependencies(m_assetPaths);
    }
}
