using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResKit
{
    public class BundleRes : ResBase
    {
        private string m_bundlePath;

        public BundleRes(string bundleName)
        {
            Name = bundleName;
            m_bundlePath = PathUtil.GetAssetBundlePath(bundleName);
            State = ResState.Loading;
        }

        public AssetBundle Bundle
        {
            get { return Asset as AssetBundle; }
            set { Asset = value; }
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;
            var req = AssetBundle.LoadFromFileAsync(m_bundlePath);
            req.completed += (operation) =>
            {
                Asset = req.assetBundle;
                State = ResState.LoadSuccess;
            };
        }

        public override bool LoadSync()
        {
            State = ResState.Loading;
            Asset = AssetBundle.LoadFromFile(m_bundlePath);
            State = ResState.LoadSuccess;

            return Asset;
        }

        protected override void OnReleaseRes()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
                Bundle = null;
            }
        }
    }
}

