namespace BuildBundle.Editor
{
    public class DontPackAssets:AssetCategory
    {
        public DontPackAssets(string assetFolder, string filter, string outputFolder) 
            : base(assetFolder, filter, outputFolder)
        {
        }

        protected override Bundle[] GetAssetBundleItems()
        {
            string[] assetPaths = GetAssets();
            Bundle[] bundles = new Bundle[assetPaths.Length];
            for (int i = 0,iMax = assetPaths.Length; i < iMax; i++)
            {
                bundles[i] = new DontPackAsset(SrcFolder, assetPaths[i], OutputFolder);
            }

            return bundles;
        }

        public void Build(string outputFolder)
        {
            for (int i = 0,iMax=ItemArray.Length; i < iMax; i++)
            {
                DontPackAsset asset = ItemArray[i] as DontPackAsset;
                asset.Build(outputFolder);
            }
        }
    }
}