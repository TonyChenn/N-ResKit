using System.IO;
using ResKit.Editor;
using UnityEditor;

namespace BuildBundle.Editor
{
    public class UIPrefabs : AssetCategory
    {
        public const string TempFolder = "Assets/N-BuildBundle/Editor/Temp";

        public UIPrefabs(string assetFolder, string filter, string outputFolder)
            : base(assetFolder, "t:Prefab", outputFolder)
        {
        }

        #region override

        protected override Bundle[] GetAssetBundleItems()
        {
            string[] subFolders = Directory.GetDirectories(base.SrcFolder);
            Bundle[] array = new Bundle[subFolders.Length];

            for (int i = 0; i < subFolders.Length; i++)
            {
                string subFolder = subFolders[i];
                string[] assets = BuildBundleHelper.GetAssetsPath(subFolder, base.Filter);
                array[i] = new UIPrefab(SrcFolder, subFolder, assets, base.OutputFolder);
            }

            return array;
        }

        public override void PrepareBuild()
        {
            base.PrepareBuild();
            foreach (Bundle item in ItemArray)
            {
                if (item.NeedBuild && item is UIPrefab prefab)
                    prefab.PrepareBuild();
            }
        }

        public override void Dispose()
        {
            AssetDatabase.DeleteAsset(TempFolder);
        }

        #endregion
    }
}