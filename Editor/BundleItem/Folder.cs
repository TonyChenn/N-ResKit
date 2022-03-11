using NCore.Editor;
using System;
using System.IO;

namespace BuildBundle.Editor
{
    public class Folder:Bundle
    {
        private string[] mAssetPathArray;
        private string mName;
        
        public Folder(string folderPath, string[] assetPaths, string outputFolder) : base(outputFolder)
        {
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets"))
                throw new ArgumentException("folderPath");
            if (assetPaths == null)
                throw new ArgumentNullException(nameof(assetPaths));
            if (assetPaths.Length < 1)
                throw new ArgumentException("assetPaths.Length < 1");

            mAssetPathArray = assetPaths;
            mName = Path.GetFileNameWithoutExtension(folderPath);
        }

        public override string ComputeHash()
        {
            return MD5Helper.GetFilesWithDependencies(mAssetPathArray);
        }

        public override string Name
        {
            get { return mName; }
        }
        public override string[] AssetNames
        {
            get { return mAssetPathArray; }
        }
    }
}