using N_AssetBundle.Runtime;
using UnityEngine;

namespace ResKit
{
    public class ResourcesRes:ResBase
    {
        private string mAssetPath;
        
        public ResourcesRes(string assetPath)
        {
            Name = assetPath;
            mAssetPath = assetPath.Substring("resources://".Length);
            State = ResState.Waiting;
        }

        #region override
        public override bool LoadSync()
        {
            State = ResState.Loading;
            Asset = Resources.Load(mAssetPath);
            State = ResState.LoadSuccess;
            return Asset;
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;
            var request = Resources.LoadAsync(mAssetPath);
            request.completed += (operation) =>
            {
                Asset = request.asset;
                State = ResState.LoadSuccess;
            };
        }

        protected override void OnReleaseRes()
        {
            if (Asset is GameObject)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                Resources.UnloadAsset(Asset);
            }
            
            ResManager.AllLoadedRes.Remove(mAssetPath);
            Asset = null;
        }
        #endregion
    }
}