using System.IO;
using UnityEditor;

namespace AssetBundle
{
    public class UIPrefabs : AssetCategory
    {
        public const string TempFolder = "Assets/Editor/AssetBundle/Temp";


        public UIPrefabs(string srcFolder, string outputFolder)
            : base(srcFolder, "t:Prefab", outputFolder)
        {
        }

        protected override BuildingBundle[] GetAssetBundleItems()
        {
            string[] subFolders = Directory.GetDirectories(base.SrcFolder);
            BuildingBundle[] array = new BuildingBundle[subFolders.Length];

            for (int i = 0; i < subFolders.Length; i++)
            {
                string subFolder = subFolders[i];
                string[] assets = GetAssets(subFolder, base.Filter);
                array[i] = new UIPrefab(base.SrcFolder, subFolder, assets, base.OutputFolder);
            }

            return array;
        }

        public override void PrepareBuild()
        {
            base.PrepareBuild();

            foreach (UIPrefab item in base.Items)
            {
                if (item.NeedBuild)
                    item.PrepareBuild();
            }
        }

        public override void Dispose()
        {
            AssetDatabase.DeleteAsset(TempFolder);
        }
    }
}
