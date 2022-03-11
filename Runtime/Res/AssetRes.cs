using N_AssetBundle.Runtime;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ResKit
{
    public class AssetRes : ResBase
    {
        private string mBundleName;
        private ResLoader loader = new ResLoader();

        public AssetRes(string bundleName, string assetName)
        {
            mBundleName = bundleName;
            Name = assetName;
            State = ResState.Waiting;
        }

        public override bool LoadSync()
        {
            State = ResState.Loading;
            var bundle = loader.LoadSync<AssetBundle>(mBundleName);
            if (ResManager.UseLocalAsset)
            {
#if UNITY_EDITOR
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(mBundleName, Name);
                Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UObject>(assetPaths[0]);
#endif
            }
            else
            {
                Asset = bundle.LoadAsset(Name);
            }

            State = ResState.LoadSuccess;
            return Asset;
        }

        public override void LoadAsync()
        {
            State = ResState.Loading;
            loader.LoadAsync<AssetBundle>(mBundleName, (bundle) =>
            {
                if (ResManager.UseLocalAsset)
                {
#if UNITY_EDITOR
                    var assetPaths =
                        UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(mBundleName, Name);
                    Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UObject>(assetPaths[0]);
                    State = ResState.LoadSuccess;
#endif
                }
                else
                {
                    var req = bundle.LoadAssetAsync(Name);
                    req.completed += (operation) =>
                    {
                        Asset = req.asset;
                        State = ResState.LoadSuccess;
                    };
                }
            });
        }

        protected override void OnReleaseRes()
        {
            if (Asset is GameObject)
            {
                
            }
            else
            {
                Resources.UnloadAsset(Asset);
            }
            loader.ReleaseAll();
            
            Asset = null;
            loader = null;

            ResManager.AllLoadedRes.Remove(Name);
        }
    }
}