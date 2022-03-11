using UnityEngine;

namespace ResKit
{
    public class BundleRes : ResBase
    {
        private string mAssetPath;

        public AssetBundle Bundle
        {
            get { return Asset as AssetBundle; }
            protected set { Asset = value; }
        }

        public BundleRes(string bundleName)
        {
            Name = bundleName;
            mAssetPath = BundlePathUtil.GetAssetBundlePath(bundleName);
            State = ResState.Loading;
        }

        #region override

        public override bool LoadSync()
        {
            State = ResState.Loading;
            Bundle = AssetBundle.LoadFromFile(mAssetPath);
            State = ResState.LoadSuccess;
            return Bundle;
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;
            var request = AssetBundle.LoadFromFileAsync(mAssetPath);
            request.completed += (operation) =>
            {
                Asset = request.assetBundle;
                State = ResState.LoadSuccess;
            };
        }

        protected override void OnReleaseRes()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
                Bundle = null;
            }
        }

        #endregion
    }
}