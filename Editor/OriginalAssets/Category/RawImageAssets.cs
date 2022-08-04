using UnityEngine.UI;

namespace AssetBundle
{
    public class RawImageAssets : NormalAssets<RawImage>
    {
        public RawImageAssets(string srcFolder, string filter, string outputFolder)
            : base(srcFolder, filter, outputFolder)
        {

        }

        protected override BuildingBundle[] GetAssetBundleItems()
        {
            string[] assetPaths = base.GetAssets();
            BuildingBundle[] items = new BuildingBundle[assetPaths.Length];

            for (int i = 0; i < assetPaths.Length; i++)
                items[i] = new RawImageAsset(assetPaths[i], base.OutputFolder, base.SrcFolder);

            return items;
        }
    }
}
