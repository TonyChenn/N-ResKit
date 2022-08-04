using System.IO;
using UObject = UnityEngine.Object;

namespace AssetBundle
{
    public class NoPackAsset : SingleFile<UObject>
    {
        string m_assetPath;
        string m_outputFolder;

        public NoPackAsset(string assetPath, string outputFolder, string srcFolder)
            : base(assetPath, outputFolder, srcFolder)
        {
            m_assetPath = assetPath;
            m_outputFolder = outputFolder;
        }

        public void Build(string productFolder)
        {
            string name = Path.GetFileName(m_assetPath);
            string targetPath = string.Format("{0}/{1}/{2}", productFolder, m_outputFolder.ToLower(), name.ToLower());
            FolderHelper.CreateFileFolder(targetPath);
            File.Delete(targetPath);
            File.Copy(m_assetPath, targetPath);
        }

        public override string[] AssetNames
        {
            get { return null; }
        }

        public override string Ext
        {
            get { return Path.GetExtension(m_assetPath); }
        }
    }
}

