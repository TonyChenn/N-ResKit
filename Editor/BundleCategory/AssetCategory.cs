using System;
using System.Collections.Generic;
using UnityEditor;

namespace BuildBundle.Editor
{
    /// <summary>
    /// 资源分类
    /// </summary>
    public abstract class AssetCategory
    {
        private string outputPath;
        private Bundle[] itemArray;
        
        #region Properties

        public string SrcFolder { get; private set; }
        public string Filter { get; private set; }
        public string OutputFolder
        {
            get { return outputPath; }
            set { outputPath = value.TrimStart('/').TrimEnd('/').ToLower(); }
        }
        
        public Bundle[] ItemArray
        {
            get
            {
                if (itemArray == null)
                    itemArray = GetAssetBundleItems();
                return itemArray;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcFolder">资源所在项目目录</param>
        /// <param name="filter">过滤器，其中如f:*.*是自定义的过滤器，做为SearchPattern搜索匹配文件</param>
        /// <param name="outputFolder">资源输出目录</param>
        public AssetCategory(string srcFolder, string filter, string outputFolder)
        {
            if (string.IsNullOrEmpty(srcFolder) || !srcFolder.StartsWith("Assets"))
                throw new ArgumentException("assetFolder");
            
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter");
            
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("outputFolder");

            SrcFolder = srcFolder;
            Filter = filter;
            OutputFolder = outputFolder;
        }

        /// <summary>
        /// 资源是否有变化
        /// </summary>
        public bool HasBundleChanged
        {
            get
            {
                if (ItemArray != null)
                {
                    foreach (var item in ItemArray)
                    {
                        if (item.LastHash != item.CurrentHash)
                            return true;
                    }
                }
                return false;
            }
        }


        public IEnumerable<AssetBundleBuild> AssetBundleBuilds
        {
            get
            {
                foreach (var item in ItemArray)
                {
                    if (item.NeedBuild)
                    {
                        yield return new AssetBundleBuild()
                        {
                            assetBundleName = item.AssetBundleName,
                            assetNames = item.AssetNames,
                        };
                    }
                }
            }
        }
        

        /// <summary>
        /// 获取指定目录的资源
        /// </summary>
        /// <returns></returns>
        protected string[] GetAssets()
        {
            return BuildBundleHelper.GetAssetsPath(SrcFolder, Filter);
        }

        #region abstract
        protected abstract Bundle[] GetAssetBundleItems();
        #endregion
        
        #region override
        public override string ToString()
        {
            return $"{SrcFolder}\t{Filter}\t{OutputFolder}";
        }

        #endregion

        #region virtual
        public virtual void Dispose()
        {
            foreach (var item in ItemArray)
            {
                item.Dispose();
            }
        }

        public virtual void ComputeHash()
        {
            foreach (var item in ItemArray)
                item.ComputeHashIfNeeded();
        }

        // 只有此资源需要打包，此方法才会执行
        public virtual void PrepareBuild()
        {
        }
        // 只有此资源需要打包，此方法才会执行
        public virtual void OnBuildFinished()
        {
        }
        // 此方法总会执行
        public virtual void OnBeforeComputeHash()
        {
        }
        // 此方法总会执行
        public virtual void OnAfterAllBuildComplete()
        {
        }
        #endregion
    }
}