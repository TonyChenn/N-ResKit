using System;
using System.IO;

namespace AssetBundle
{
    public class Folder : BuildingBundle
    {
        string[] m_assetPaths;
        string m_name;

        public Folder(string folderPath, string[] assetPaths, string outputFolder)
            : base(outputFolder)
        {
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets"))
                throw new ArgumentException("folderPath");
            if (assetPaths == null)
                throw new ArgumentNullException("assetPaths");
            if (assetPaths.Length < 1)
                throw new ArgumentException("assetPaths.Length < 1");

            m_assetPaths = assetPaths;
            m_name = Path.GetFileNameWithoutExtension(folderPath);
        }

        protected override string ComputeHash()
        {
            return ComputeHashWithDependencies(m_assetPaths);
        }

        public override string Name
        {
            get { return m_name; }
        }

        public override string[] AssetNames
        {
            get { return m_assetPaths; }
        }
    }
}
