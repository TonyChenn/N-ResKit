using UObject = UnityEngine.Object;
using System;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace AssetBundle
{
    public abstract class AssetCategory
    {
        string m_outputFolder;
        BuildingBundle[] m_items;

        protected abstract BuildingBundle[] GetAssetBundleItems();


        #region Properties

        public string SrcFolder
        {
            private set;
            get;
        }

        public string Filter
        {
            private set;
            get;
        }

        public string OutputFolder
        {
            get { return m_outputFolder; }
            set { m_outputFolder = value.TrimStart('/').TrimEnd('/').ToLower(); }
        }

        public BuildingBundle[] Items
        {
            get
            {
                if (m_items == null)
                    m_items = GetAssetBundleItems();
                return m_items;
            }
        }

        public bool ExistChangeItem
        {
            get
            {
                if (Items != null)
                {
                    foreach (var item in Items)
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
                foreach (var item in Items)
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

        #endregion


        #region static

        /// <summary>
        /// 获取指定目录的资源
        /// </summary>
        /// <param name="filter">过滤器，若以t:开头，表示用unity的方式过滤；若以f:开头，表示用windows的SearchPattern方式过滤；若以r:开头，表示用正则表达式的方式过滤。</param>
        public static string[] GetAssets(string folder, string filter)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentException("folder");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter");

            if (filter.StartsWith("t:"))
            {
                string[] guids = AssetDatabase.FindAssets(filter, new string[] { folder });
                string[] paths = new string[guids.Length];
                for (int i = 0; i < guids.Length; i++)
                    paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                return paths;
            }
            else if (filter.StartsWith("f:"))
            {
                string folderFullPath = BuildingBundle.GetFullPath(folder);
                string searchPattern = filter.Substring(2);
                string[] files = Directory.GetFiles(folderFullPath, searchPattern, SearchOption.AllDirectories);
                string[] paths = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                    paths[i] = BuildingBundle.GetAssetPath(files[i]);
                return paths;
            }
            else if (filter.StartsWith("r:"))
            {
                string folderFullPath = BuildingBundle.GetFullPath(folder);
                string pattern = filter.Substring(2);
                string[] files = Directory.GetFiles(folderFullPath, "*.*", SearchOption.AllDirectories);
                List<string> list = new List<string>();
                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileName(files[i]);
                    if (Regex.IsMatch(name, pattern))
                    {
                        string p = BuildingBundle.GetAssetPath(files[i]);
                        list.Add(p);
                    }
                }
                return list.ToArray();
            }
            else
            {
                throw new InvalidOperationException("Unexpected filter: " + filter);
            }
        }

        #endregion


        /// <summary>
        /// 资源类别，如lua是一个类别，ui prefab是一个类别
        /// </summary>
        /// <param name="srcFolder">资源所在项目目录</param>
        /// <param name="filter">过滤器，其中如f:*.*是自定义的过滤器，做为SearchPattern搜索匹配文件</param>
        /// <param name="outputFolder">此资源输出的目录</param>
        public AssetCategory(string srcFolder, string filter, string outputFolder)
        {
            if (string.IsNullOrEmpty(srcFolder) || !srcFolder.StartsWith("Assets"))
                throw new ArgumentException("srcFolder");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter");
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("outputFolder");

            SrcFolder = srcFolder;
            Filter = filter;
            OutputFolder = outputFolder;
        }

        public virtual void ComputeHash()
        {
            foreach (var item in Items)
                item.ComputeHashIfNeeded();
        }

        protected string[] GetAssets()
        {
            return GetAssets(SrcFolder, Filter);
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", SrcFolder, Filter, OutputFolder);
        }

        public virtual void Dispose()
        {
            foreach (var item in Items)
                item.Dispose();
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


    }
}
