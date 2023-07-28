using UObject = UnityEngine.Object;


public class SingleFile<T> : BaseBundle where T : UObject
{
    private string m_srcFolder;
    protected string m_assetPath;
    string m_nameToSrcFolder;

    protected string SrcFolder { get { return m_srcFolder; } }

    public string AssetPath { get { return m_assetPath; } }

    public override string Name
    {
        get
        {
            if (string.IsNullOrEmpty(m_nameToSrcFolder))
                m_nameToSrcFolder = GetNameToSrcFolder(m_assetPath, m_srcFolder);
            return m_nameToSrcFolder;
        }
    }

    public override string[] AssetNames => new string[] { m_assetPath };

    public SingleFile(string assetPath, string outputFolder, string srcFolder)
        : base(outputFolder)
    {
        m_srcFolder = srcFolder.Replace("\\", "/").TrimEnd('/');
        m_assetPath = assetPath.Replace("\\", "/").TrimEnd('/');
    }

    public static string GetNameToSrcFolder(string assetPath, string srcFolder)
    {
        srcFolder = srcFolder.Replace("\\", "/").TrimEnd('/');
        int startIndex = srcFolder.Length + 1;
        int lastIndex = assetPath.LastIndexOf(".");
        int length = lastIndex - startIndex;
        return assetPath.Substring(startIndex, length);
    }
}
