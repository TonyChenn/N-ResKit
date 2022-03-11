using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BuildBundle.Editor;
using NCore;
using NCore.Editor;
using UnityEditor;

namespace ResKit.Editor
{
    public static class BuildAB
    {
        /// <param name="outputPath">AB包输出目录</param>
        /// <param name="deleteOld">是否删除老的AB包</param>
        public static IEnumerator Build(string outputPath, bool rebuildAll = false, bool deleteOld = false)
        {
            string buildHashFile = $"{outputPath}/main_hash.csv";       // hash
            string buildUpdateFile = $"{outputPath}/update.dat";        // 客户端要更新的信息
            string versionFile = $"{outputPath}/version.dat";           // 版本信息
            string tempFolder = $"{outputPath}/temp";                   // temp Folder


            yield return ">>>>>>>>>>开始打包...";
            // 读取打包资源列表配置
            List<AssetCategory> assetCategories = BuildConfig.GetNeedBuildAssetList();
            if (assetCategories==null || assetCategories.Count < 1)
            {
                string info = ">>>>>>>>>>没有要打包的资源，请检查";
                yield return info;
                EditorUtility.DisplayDialog("Error", info, "好的");
                yield break;
            }

            int assetCount = assetCategories.Count;

            // 读取上次打包结果
            string[] lastBuildHash = null;
            
            if (rebuildAll)
                File.Delete(buildHashFile);
            else
            {
                if (File.Exists(buildHashFile))
                {
                    lastBuildHash = File.ReadAllLines(buildHashFile);
                    for (int i = 0; i < assetCount; i++)
                    {
                        foreach (var item in assetCategories[i].ItemArray)
                            item.ReadConfig(lastBuildHash);
                    }
                }
                else
                {
                    yield return ">>>>>>>>>>首次打包...";
                }
            }

            for (int i = 0; i < assetCount; i++)
                assetCategories[i].OnBeforeComputeHash();


            yield return ">>>>>>>>>>重新计算资源Hash...";
            for (int i = 0; i < assetCount; i++)
                assetCategories[i].ComputeHash();

            // 判断有没有资源发生了变化（是否需要打包）
            bool needBuild = false;
            for (int i = 0; i < assetCount; i++)
            {
                if (assetCategories[i].HasBundleChanged)
                {
                    needBuild = true;
                    break;
                }
            }

            UpdateAssetState(assetCategories);
            // 获取要删除的资源
            List<string> deleteList;
            CalcuteNeedRemoveAsset(assetCategories, lastBuildHash, out deleteList);
            //开始打包
            if (needBuild)
            {
                yield return ">>>>>>>>>>开始打包到临时目录...";
                BuildToTemp(tempFolder, assetCategories);

                // 检查打包是否正常

                yield return ">>>>>>>>>>正在移动到目标目录...";
                CopyToTargetFolder(tempFolder, outputPath);
                yield return ">>>>>>>>>>打包完成...";

                foreach (var c in assetCategories)
                {
                    c.OnAfterAllBuildComplete();
                }

                // 资源文件变更信息
                var log = StringBuilderPool.Alloc();
                bool versionChanged = CheckVersionChanged(assetCategories, deleteList, ref log);
                yield return log.ToString();
                
                yield return ">>>>>>>>>>生成版控文件...";
                string str_version = string.Empty;
                if (versionChanged)
                {
                    SaveBundleResultHash(buildHashFile, assetCategories);
                    str_version = VersionHelper.GenNewVersionString();
                    File.WriteAllText(versionFile, str_version);
                }
                yield return "最新版本号：" + str_version;

                int changedFileBytes = SaveClientDownloadInfo(outputPath, buildUpdateFile, assetCategories, ref log);
                int kb = changedFileBytes / 1024;
                int mb = kb / 1024;
                int leftKB = kb % 1024;
                string sizeLog = string.Format("发生变化（新增或修改）的ab包大小：{0} MB {1} KB", mb, leftKB);
                
                yield return sizeLog;

                // 日志信息
                // TODO 保存日志信息
                
                // 删除要删除的AB包
                DeleteRemovedAssetBundles(outputPath, deleteList);

                // dispose
                foreach (var c in assetCategories)
                    c.Dispose();
                
                // 删除temp文件夹
                Directory.Delete(tempFolder,true);
            }
            else
            {
                yield return "没有资源发生变更";
            }
        }
        #region methods

        /// <summary>
        /// 打包到临时文件夹
        /// </summary>
        static void BuildToTemp(string tempFolder, List<AssetCategory> categories, bool deleteOld = true)
        {
            if (Directory.Exists(tempFolder) && deleteOld)
                Directory.Delete(tempFolder,true);
            Directory.CreateDirectory(tempFolder);

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            foreach (AssetCategory c in categories)
            {
                if (c.HasBundleChanged)
                {
                    if (c is DontPackAssets asset)
                    {
                        asset.Build(tempFolder);
                    }
                    else
                    {
                        c.PrepareBuild();
                        list.AddRange(c.AssetBundleBuilds);
                    }
                }
            }

            AssetDatabase.Refresh();

            BuildPipeline.BuildAssetBundles(tempFolder, list.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);

            // 删除manifest
            string[] manifestFiles = Directory.GetFiles(tempFolder, "*.manifest", SearchOption.AllDirectories);
            foreach (string manifest in manifestFiles)
                File.Delete(manifest);
            
            // 删除文件夹文件
            string folderFile = $"{tempFolder}/{Path.GetFileNameWithoutExtension(tempFolder)}";
            File.Delete(folderFile);
        }
        
        /// <summary>
        /// 更新资源状态
        /// </summary>
        static void UpdateAssetState(List<AssetCategory> categories)
        {
            for (int i = 0, iMax = categories.Count; i < iMax; i++)
            {
                foreach (var item in categories[i].ItemArray)
                {
                    if (string.IsNullOrEmpty(item.LastHash))
                        item.BuildFlag = Bundle.Flag.NewAdded;
                    else if (item.LastHash != item.CurrentHash)
                        item.BuildFlag = Bundle.Flag.Modified;
                    else
                        item.BuildFlag = Bundle.Flag.NoChange;
                }
            }
        }
        
        /// <summary>
        /// 从临时文件夹复制到目标文件夹
        /// </summary>
        static void CopyToTargetFolder(string fromFolder, string targetFolder)
        {
            string[] files = Directory.GetFiles(fromFolder, "*.*", SearchOption.AllDirectories);
            for (int i = 0, iMax = files.Length; i < iMax; i++)
            {
                string relativePath = files[i].Substring(fromFolder.Length);
                string targetFile = $"{targetFolder}{relativePath}";
                string folder = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                File.Copy(files[i], targetFile, true);
            }
        }

        /// <summary>
        /// 计算要移除的资源
        /// </summary>
        static void CalcuteNeedRemoveAsset(List<AssetCategory> categories, string[] oldHashLine,
            out List<string> deleteList)
        {
            deleteList = new List<string>();
            if (oldHashLine == null) return;

            foreach (string line in oldHashLine)
            {
                bool removed = true;
                foreach (AssetCategory ca in categories)
                {
                    foreach (Bundle item in ca.ItemArray)
                    {
                        if (line.StartsWith($"{item.OutputRelativePath}{item.Ext},"))
                        {
                            removed = false;
                            break;
                        }
                    }
                }

                if (removed)
                {
                    string relativePath;
                    Bundle.ParseConfigLine(line, out relativePath, out _);
                    deleteList.Add(relativePath);
                }
            }
        }

        /// <summary>
        /// 检查Version是否发生变化
        /// </summary>
        static bool CheckVersionChanged(List<AssetCategory> categories, List<string> removedList, ref StringBuilder log)
        {
            log.Length = 0;

            List<string> addedList = new List<string>();
            List<string> modifiedList = new List<string>();

            foreach (var c in categories)
            {
                foreach (var item in c.ItemArray)
                {
                    if (item.BuildFlag == Bundle.Flag.NewAdded)                // 新增的
                        addedList.Add(item.OutputRelativePath);
                    else if (item.BuildFlag == Bundle.Flag.Modified)             // 修改的
                        modifiedList.Add(item.OutputRelativePath);
                }
            }

            // log
            int totalCount = addedList.Count + modifiedList.Count + removedList.Count;
            if (totalCount < 1)
            {
                log.Append("无任何资源发生修改。");
                return false;
            }

            log.AppendLine(string.Format("共有{0}个包发生变化，详细如下：", totalCount));
            if (addedList.Count > 0)
            {
                log.AppendLine(string.Format("新增{0}个包：", addedList.Count));
                foreach (var item in addedList)
                    log.AppendLine(item);
            }

            if (modifiedList.Count > 0)
            {
                log.AppendLine(string.Format("有{0}个包发生修改：", modifiedList.Count));
                foreach (var item in modifiedList)
                    log.AppendLine(item);
            }

            if (removedList.Count > 0)
            {
                log.AppendLine(string.Format("移除了{0}个包：", removedList.Count));
                foreach (var item in removedList)
                    log.AppendLine(item);
            }

            return true;
        }

        /// <summary>
        /// 保存打包信息
        /// </summary>
        static void SaveBundleResultHash(string hashFilePath, List<AssetCategory> categories)
        {
            var builder = StringBuilderPool.Alloc();
            foreach (var c in categories)
            {
                foreach (var item in c.ItemArray)
                {
                    string config = item.GenConfigLine();
                    builder.AppendLine(config);
                }
            }
            File.WriteAllText(hashFilePath,builder.ToString());
            builder.Recycle();
        }

        static int SaveClientDownloadInfo(string outputFolder,string updateFilePath, List<AssetCategory> categories, ref StringBuilder sb)
        {
            sb.Length = 0;
            int changedABBytes = 0;

            // calc hash
            for (int i = 0; i < categories.Count; i++)
            {
                for (int j = 0; j < categories[i].ItemArray.Length; j++)
                {
                    var c = categories[i];
                    var item = c.ItemArray[j];
                    string name = item.OutputRelativePath + item.Ext;
                    bool changed = item.BuildFlag != Bundle.Flag.NoChange;
                    string abPath = string.Format("{0}/{1}", outputFolder, name);
                    byte[] bytes = File.ReadAllBytes(abPath);
                    string hash = MD5Helper.ComputeHash(bytes);

                    string strDownloadType = "true";

                    sb.AppendFormat("{0},{1},{2},{3};", name, hash, bytes.Length, strDownloadType);

                    if (changed)
                        changedABBytes += bytes.Length;
                }
            }
            File.WriteAllText(updateFilePath, sb.ToString());

            return changedABBytes;
        }
        
        static void DeleteRemovedAssetBundles(string outputFolder, List<string> removedList)
        {
            for (int i = 0; i < removedList.Count; i++)
            {
                // 从磁盘中删除
                string fullPath = string.Format($"{outputFolder}/{removedList[i]}");
                if (Directory.Exists(Path.GetDirectoryName(fullPath)))
                    File.Delete(fullPath);
            }
        }
        #endregion
    }
}