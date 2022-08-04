using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace AssetBundle
{
    public static class BuildingConfig
    {
        public static BuildTarget SelectedBuildTarget;

        public enum Project
        {
            Main = 0
        }


        #region Properties

        public static Project CurrentProject { get { return Project.Main; } }

        public static string PlatformFolderName
        {
            get
            {
                string folder;
                switch (SelectedBuildTarget)
                {
                    case BuildTarget.Android:
                        folder = "Android";
                        break;
                    case BuildTarget.iOS:
                        folder = "IOS";
                        break;
                    default:
                        folder = "PC";
                        break;
                }

                return folder;
            }
        }

        public static BuildAssetBundleOptions BuildingOptions
        {
            get
            {
                return CurrentProject == Project.Main ?
                    BuildAssetBundleOptions.ChunkBasedCompression :
                    BuildAssetBundleOptions.None;
            }
        }

        #endregion


        static BuildingConfig()
        {
            SelectedBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        public static List<AssetCategory> GetAssetCategories()
        {
            return new List<AssetCategory>()
            {
                new NormalAssets<UObject>("Assets/BuildBundle/Asset/Table", "f:*.asset", "table"),      // 表格
                new NormalAssets<UObject>("Assets/BuildBundle/Asset/Config", "f:*.asset", "config"),    // 配置
                new NormalAssets<Texture>("Assets/BuildBundle/UI/Image", "t:Texture2D", "image"),       // image
                new UIPrefabs("Assets/BuildBundle/UI/Prefabs", "ui/prefabs"),                           // UIPrefab

                new NormalAssets<UObject>("Assets/UI/Atlas/common", "t:Prefab", "UI"),                                         // 通用图集
                new NormalAssets<UObject>("Assets/UI/Font", "t:Prefab", "UI"),                                                // 通用字体
                new TextFiles("Assets/Config/Txt", "f:*.txt", "txt"),                                                          // txt文件
                new TextFiles("Assets/Config/CSV/NoviceGuide", "f:*.csv", "data"),                                             // 新手引导csv文件
            };
        }
    }
}
