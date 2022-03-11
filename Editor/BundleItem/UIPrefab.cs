using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using NCore;
using UObject = UnityEngine.Object;
using NCore.Editor;

namespace BuildBundle.Editor
{
    public class UIPrefab : Bundle
    {
        private readonly string mSrcFolder;
        private string[] mAssetPaths;
        private string mName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFolder">UIPrefab总目录</param>
        /// <param name="folderPath">UIPrefab文件夹中的子文件夹(每个子文件夹为AB包)</param>
        /// <param name="assetPaths">folderPath下资源</param>
        /// <param name="outputFolder"></param>
        public UIPrefab(string srcFolder, string folderPath, string[] assetPaths, string outputFolder)
            : base(outputFolder)
        {
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets"))
                throw new ArgumentException("folderPath");
            if (assetPaths == null)
                throw new ArgumentNullException("assetPaths");
            if (assetPaths.Length < 1)
                throw new ArgumentException(string.Format("assetPaths.Length < 1, folderPath: {0}", folderPath));

            mSrcFolder = srcFolder;
            mAssetPaths = assetPaths;
            mName = Path.GetFileNameWithoutExtension(folderPath);
        }

        #region override
        public override string ComputeHash()
        {
            return MD5Helper.GetFilesWithDependencies(mAssetPaths);
        }

        public override string Name => mName;

        public override string[] AssetNames
        {
            get
            {
                if (!NeedBuild) return null;
                if (mAssetPaths.Length == 0)
                {
                    Debug.LogError($"No asset{Name}");
                    return null;
                }

                string[] tempAssets = new string[mAssetPaths.Length];
                for (int i = 0, iMax = mAssetPaths.Length; i < iMax; i++)
                {
                    tempAssets[i] = mAssetPaths[i].Replace(mSrcFolder, UIPrefabs.TempFolder);
                }

                return tempAssets;
            }
        }
        #endregion


        public void PrepareBuild()
        {
            if (mAssetPaths.Length < 0)
            {
                Debug.LogError("No asset: " + Name);
                return;
            }

            for (int i = 0; i < mAssetPaths.Length; i++)
            {
                string assetPath = mAssetPaths[i];
                GameObject obj = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
                
                var temp = UObject.Instantiate(obj);
                // 复制一份出来
                string newPath = assetPath.Replace(mSrcFolder, UIPrefabs.TempFolder);
                newPath.CreateFileFolder();
                GameObject tempObj = PrefabUtility.SaveAsPrefabAsset(temp, newPath);
                UObject.DestroyImmediate(temp);
                // 本地化图片字脚本（一定要在去掉 common 图集之前）
                // 去掉 common 图集
                // 去掉通用字体
                // 本地化文字脚本
            }
        }
    }
}