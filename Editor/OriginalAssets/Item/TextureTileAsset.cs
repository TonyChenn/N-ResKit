using System.IO;
using UnityEngine.UI;

namespace AssetBundle
{
    public class RawImageAsset : SingleFile<RawImage>
    {
        public RawImageAsset(string assetPath, string outputFolder, string srcFolder)
            : base(assetPath, outputFolder, srcFolder)
        {

        }

        public override string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(m_assetPath);
            }
        }
    }
}
