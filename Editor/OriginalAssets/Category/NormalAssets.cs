using UObject = UnityEngine.Object;

namespace AssetBundle
{
    public class NormalAssets<T> : AssetCategory where T : UObject
    {
        public NormalAssets(string srcFolder, string filter, string outputFolder)
            : base(srcFolder, filter, outputFolder)
        {

        }

        protected override BuildingBundle[] GetAssetBundleItems()
        {
            string[] assetPaths = base.GetAssets();
            BuildingBundle[] items = new BuildingBundle[assetPaths.Length];

            for (int i = 0; i < assetPaths.Length; i++)
                items[i] = new SingleFile<T>(assetPaths[i], base.OutputFolder, base.SrcFolder);

            return items;
        }
    }
}
