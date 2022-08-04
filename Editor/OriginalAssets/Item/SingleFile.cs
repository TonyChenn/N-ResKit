using System;
using UObject = UnityEngine.Object;

namespace AssetBundle
{
    public class SingleFile<T> : BuildingBundle where T : UObject
    {
        protected string m_assetPath;
        string m_srcFolder;
        string m_nameToSrcFolder;

        public static string GetNameToSrcFolder(string assetPath, string srcFolder)
        {
            srcFolder = srcFolder.Replace("\\", "/").TrimEnd('/');
            int startIndex = srcFolder.Length + 1;
            int lastIndex = assetPath.LastIndexOf(".");
            int length = lastIndex - startIndex;
            return assetPath.Substring(startIndex, length);
        }

        public SingleFile(string assetPath, string outputFolder, string srcFolder)
            : base(outputFolder)
        {
            if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets"))
                throw new ArgumentException("assetPath");

            m_assetPath = assetPath.Replace("\\", "/");
            m_srcFolder = srcFolder.Replace("\\", "/").TrimEnd('/');
        }

        protected override string ComputeHash()
        {
            return ComputeHashWithDependencies(m_assetPath);
        }

        public override string[] AssetNames
        {
            get { return new string[] {m_assetPath}; }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_nameToSrcFolder))
                    m_nameToSrcFolder = GetNameToSrcFolder(m_assetPath, m_srcFolder);
                return m_nameToSrcFolder;
            }
        }

        public string AssetPath
        {
            get { return m_assetPath; }
        }

        protected string SrcFolder
        {
            get { return m_srcFolder; }
        }
    }
}