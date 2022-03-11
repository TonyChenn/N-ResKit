using System.IO;
using NCore;
using UObject = UnityEngine.Object;

namespace BuildBundle.Editor
{
    public class DontPackAsset:SingleFile<UObject>
    {
        private string mAssetPath;
        private string mOutputFolder;
        
        public DontPackAsset(string srcFolder,string assetPath, string outputFolder) 
            : base(srcFolder, assetPath, outputFolder)
        {
            mAssetPath = assetPath;
            mOutputFolder = outputFolder;
        }

        public override string[] AssetNames => null;
        public override string Ext => Path.GetExtension(mAssetPath);

        public void Build(string outputFolder)
        {
            string name = Path.GetFileName(mAssetPath);
            string targetPath = $"{outputFolder}/{mOutputFolder.ToLower()}/{name.ToLower()}";
            
            targetPath.CreateFileFolder();
            File.Delete(targetPath);
            File.Copy(mAssetPath,targetPath);
        }
    }
}