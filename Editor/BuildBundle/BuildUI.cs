using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetBundle
{
    public static class BuildUI
    {
        public static void Build()
        {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
        }

        private static List<AssetCategory> uiPrefabsCategory()
        {
            return new List<AssetCategory>()
            {
                new UIPrefabs("Assets/BuildBundle/UI/Prefabs","ui/prefabs"),
            };
        }
    }
}
