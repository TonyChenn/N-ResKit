namespace BuildBundle.Editor
{
    public class LuaCodes : AssetCategory
    {
        public LuaCodes(string assetFolder, string filter, string outputFolder) 
            : base(assetFolder, "f:*.lua", outputFolder)
        {
        }

        protected override Bundle[] GetAssetBundleItems()
        {
            string[] luaScripts = GetAssets();
            Folder folder = new Folder(SrcFolder, luaScripts, OutputFolder);
            return new Bundle[] {folder};
        }
    }
}