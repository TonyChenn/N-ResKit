using UnityEditor;

namespace AssetBundle
{
    public class AnimationClips : AssetCategory
    {
        public AnimationClips(string srcFolder, string outputFolder)
            : base(srcFolder, "f:*.FBX", outputFolder)
        {

        }

        protected override BuildingBundle[] GetAssetBundleItems()
        {
            string[] assetPaths = base.GetAssets();
            BuildingBundle[] items = new BuildingBundle[assetPaths.Length];

            for (int i = 0; i < assetPaths.Length; i++)
                items[i] = new AnimClip(assetPaths[i], base.OutputFolder, base.SrcFolder);

            return items;
        }

        public override void Dispose()
        {
            base.Dispose();
            AssetDatabase.DeleteAsset("Assets/Resources/Temp");
        }

        public override void PrepareBuild()
        {
            base.PrepareBuild();
            foreach (AnimClip clip in base.Items)
            {
                if (clip.NeedBuild)
                    clip.GenerateClip();
            }
        }

    }
}
