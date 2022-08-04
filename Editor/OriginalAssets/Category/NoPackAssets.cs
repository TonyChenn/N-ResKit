namespace AssetBundle
{
    public class NoPackAssets : AssetCategory
    {
        public NoPackAssets(string srcFolder, string filter, string outputFolder)
            : base(srcFolder, filter, outputFolder)
        {

        }

        protected override BuildingBundle[] GetAssetBundleItems()
        {
            string[] assetPaths = base.GetAssets();
            BuildingBundle[] items = new BuildingBundle[assetPaths.Length];

            for (int i = 0; i < assetPaths.Length; i++)
                items[i] = new NoPackAsset(assetPaths[i], base.OutputFolder, base.SrcFolder);

            return items;
        }

        public void Build(string productFolder)
        {
            for (int i = 0; i < base.Items.Length; i++)
            {
                NoPackAsset item = (NoPackAsset)base.Items[i];
                item.Build(productFolder);
            }
        }
    }
}
