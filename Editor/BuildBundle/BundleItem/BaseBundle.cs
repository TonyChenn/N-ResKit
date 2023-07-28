public abstract class BaseBundle
{
    string m_outputFolder;

    public BaseBundle(string outputFolder)
    {
        m_outputFolder = outputFolder.TrimStart('/').TrimEnd('/');
    }
    public string AssetBundleName
    {
        get { return $"{m_outputFolder}/{Name}{Ext}"; }
    }

    public string FullName { get { return Name + Ext; } }


    #region absract

    public abstract string Name { get; }

    public abstract string[] AssetNames { get; }
    #endregion

    #region virtual
    public virtual void Dispose() { }
    public virtual string Ext { get { return ".u"; } }
    #endregion
}
