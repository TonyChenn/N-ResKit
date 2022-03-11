using System;
using NCore;
using NCore.Editor;
using UObject = UnityEngine.Object;

namespace BuildBundle.Editor
{
    public class SingleFile<T> : Bundle where T : UObject
    {
        protected string mAssetPath;
        private string mSrcFolder;
        private string mSrcName;
        
        public SingleFile(string srcFolder, string assetPath, string outputFolder) : base(outputFolder)
        {
            if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets"))
                throw new ArgumentException("assetPath");

            mAssetPath = assetPath.MakeStandardPath();
            mSrcFolder = srcFolder.MakeStandardPath();
        }
        
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(mSrcName))
                    mSrcName = GetSrcName(mAssetPath, mSrcFolder);
                return mSrcName;
            }
        }

        public override string[] AssetNames
        {
            get { return new[] {mAssetPath}; }
        }
        
        public string AssetPath
        {
            get { return mAssetPath; }
        }

        protected string SrcFolder
        {
            get { return mSrcFolder; }
        }

        public override string ComputeHash()
        {
            return MD5Helper.GetFileWithDependencies(mAssetPath);
        }
        
        public static string GetSrcName(string assetPath, string srcFolder)
        {
            srcFolder = srcFolder.MakeStandardPath();
            
            int startIndex = srcFolder.Length + 1;
            int lastIndex = assetPath.LastIndexOf(".", StringComparison.Ordinal);
            int length = lastIndex - startIndex;
            return assetPath.Substring(startIndex, length);
        }
    }
}