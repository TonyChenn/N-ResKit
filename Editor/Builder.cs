using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetBundle
{
    public class Builder
    {
        public const string VerionFileName = "version.dat";
        public const string ListInfoFileName = "listinfo";

        string m_targetFolder;


        #region properties

        string BuildingRootFolder
        {
            get { return m_targetFolder; }
            set { m_targetFolder = value.Replace("\\", "/").TrimEnd('/'); }
        }

        string BuildingListPath
        {
            get { return GetBuildingListPath(BuildingRootFolder); }
        }

        public static string BuildingLogPath
        {
            get { return TempFolder + "/log.txt"; }
        }

        string VersionPath
        {
            get { return GetVersionPath(BuildingRootFolder); }
        }

        string LoadingListPath
        {
            get { return GetLoadingListPath(BuildingRootFolder); }
        }

        static string ListFileName
        {
            get { return BuildingConfig.CurrentProject.ToString().ToLower(); }
        }

        public static string JenkinsBuildLogPath
        {
            get { return TempFolder + "/JenkinsBuildLog.txt"; }
        }

        static string TempFolder
        {
            get
            {
                string folder = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
                return folder + "/AssetBundles";
            }
        }

        string TempBuildingFolder
        {
            get { return TempFolder + "/Temp"; }
        }

        public string Version
        {
            get;
            private set;
        }

        public string Log
        {
            get;
            private set;
        }

        #endregion


        #region static

        public static string GetBuildingListPath(string buildingRootFolder)
        {
            return string.Format("{0}/hash/{1}.csv", buildingRootFolder, ListFileName);
        }

        public static string GetVersionPath(string buildingRootFolder)
        {
            return string.Format("{0}/{1}", buildingRootFolder, VerionFileName);
        }

        public static string GetLoadingListPath(string buildingRootFolder)
        {
            return GetLoadingListPath(buildingRootFolder, BuildingConfig.CurrentProject);
        }

        public static List<int> ReadVersion(string versionPath)
        {
            List<int> list = new List<int>();

            string path = versionPath;
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                string[] words = text.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < words.Length; i++)
                {
                    int v;
                    int.TryParse(words[i], out v);
                    list.Add(v);
                }
            }

            return list;
        }

        public static string VersionIncrease(string versionPath)
        {
            List<int> versions = ReadVersion(versionPath);
            int index = (int)BuildingConfig.CurrentProject;

            if (index > versions.Count)
            {
                throw new InvalidOperationException("index > versions.Count");
            }
            else if (index == versions.Count)       // 这是第一次打包的情况
            {
                versions.Add(1);
            }
            else
            {
                versions[index]++;
            }

            string str = CombineVersionString(versions);
            string path = versionPath;

            File.WriteAllText(path, str);

            return str;
        }

        static string CombineVersionString(List<int> listVersion)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in listVersion)
            {
                sb.AppendFormat(".{0}", v);
            }
            string str = sb.ToString().Trim('.');
            return str;
        }

        /*
        public static void ReGenerateListFiles()
        {
            string[] files = { "art.dat", "main.dat", "music.dat" };
            string folder = "Assets/StreamingAssets/";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                sb.Length = 0;
                string name = files[i];
                string path = folder + name;
                string content = File.ReadAllText(path);
                string[] lines = content.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string abName = words[0];
                    string abPath = folder + abName;
                    byte[] bytes = File.ReadAllBytes(abPath);
                    string hash = BuildingBundle.ComputeHash(bytes);

                    sb.AppendFormat("{0},{1},{2};", abName, hash, bytes.Length);
                }
                File.WriteAllText(path, sb.ToString());

            }
            UnityEngine.Debug.Log("Done!");
        }*/

        public static void OpenLogFile()
        {
            string folder = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
            string path = string.Format("{0}/AssetBundles/log.txt", folder);
            if (File.Exists(path))
            {
                Process.Start(path);
            }
        }

        static string GetLoadingListPath(string folder, BuildingConfig.Project pro)
        {
            string fileName = pro.ToString().ToLower();
            return GetDatFilePath(folder, fileName);
        }

        static string GetDatFilePath(string folder, string fileName)
        {
            return string.Format("{0}/{1}.dat", folder, fileName);
        }

        public static void SaveListInfoFile(string folder)
        {
            // main;art;music
            StringBuilder sb = new StringBuilder();
            Array array = Enum.GetValues(typeof(BuildingConfig.Project));
            for (int i = 0; i < array.Length; i++)
            {
                var proj = (BuildingConfig.Project)array.GetValue(i);
                string path = GetLoadingListPath(folder, BuildingConfig.Project.Main);
				if (!File.Exists(path))
                {
                    return;
                }
				
                byte[] buffer = File.ReadAllBytes(path);
                int size = buffer.Length;
                sb.AppendFormat(";{0}", size);
            }

            string str = sb.ToString().Trim(';');
            string savePath = GetDatFilePath(folder, ListInfoFileName);

            File.WriteAllText(savePath, str);
        }

        static void CopyFiles(string fromDir, string toDir)
        {
            string[] files = Directory.GetFiles(fromDir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string relativePath = files[i].Substring(fromDir.Length);
                string targetFile = string.Format("{0}{1}", toDir, relativePath);
                FolderHelper.CreateFileFolder(targetFile);
                File.Copy(files[i], targetFile, true);
            }
        }

        // 打ab包
        public static void UnityBuild(string folder, List<AssetCategory> categories, bool deleteOld = true)
        {
            if (deleteOld && Directory.Exists(folder))
                Directory.Delete(folder, true);
            Directory.CreateDirectory(folder);

            // 准备打包
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            foreach (var c in categories)
            {
                if (c.ExistChangeItem)
                {
                    var noPackAssets = c as NoPackAssets;
                    if (noPackAssets != null)
                    {
                        // 无需打包的资源
                        noPackAssets.Build(folder);
                    }
                    else
                    {
                        c.PrepareBuild();
                        list.AddRange(c.AssetBundleBuilds);
                    }
                }
            }
            AssetDatabase.Refresh();

            BuildPipeline.BuildAssetBundles(folder, list.ToArray(), BuildingConfig.BuildingOptions, BuildingConfig.SelectedBuildTarget);
            // delete manifest files
            string[] manifestFiles = Directory.GetFiles(folder, "*.manifest", SearchOption.AllDirectories);
            foreach (var mf in manifestFiles)
                File.Delete(mf);
            string folderFile = string.Format("{0}/{1}", folder, Path.GetFileNameWithoutExtension(folder));
            File.Delete(folderFile);
        }

        static bool AllAssetBundleBuildedSucceed(List<AssetCategory> categories, string folder)
        {
            bool succeed = true;
            foreach (var c in categories)
            {
                foreach (var item in c.Items)
                {
                    if (item.BuildingFlag != BuildingBundle.Flag.NoChange)
                    {
                        string path = string.Format("{0}/{1}{2}", folder, item.OutputRelativePath, item.Ext);
                        bool exist = File.Exists(path);
                        if (!exist)
                        {
                            succeed = false;
                            break;
                        }
                    }
                }
            }

            return succeed;
        }

        #endregion


        public IEnumerator Build(string targetFolder, bool rebuildAll)
        {
            BuildingRootFolder = targetFolder;

            yield return "开始打包...";

            // 1. 读取打包资源列表配置
            List<AssetCategory> categories = BuildingConfig.GetAssetCategories();
            if (categories == null || categories.Count < 1)
            {
                string str = "无任何要打包的资源，请检查配置是否正确。";
                yield return str;
                EditorUtility.DisplayDialog("错误", str, "确定");
                yield break;
            }

            // 2. 读取上次打包结果
            string[] lastBuildConfig = null;
            if (rebuildAll)
            {
                if (File.Exists(BuildingListPath))
                    File.Delete(BuildingListPath);

                SetVersion(0);
            }
            else
            {
                if (File.Exists(BuildingListPath))
                {
                    lastBuildConfig = File.ReadAllLines(BuildingListPath);
                    foreach (var c in categories)
                    {
                        foreach (var item in c.Items)
                            item.ReadConfig(lastBuildConfig);
                    }
                }
                else
                    yield return "这是第一次打包。";
            }

            //
            foreach (var c in categories)
            {
                c.OnBeforeComputeHash();
            }

            // 3. 计算资源的哈希值...
            yield return "计算资源的哈希值...";
            foreach (var c in categories)
                c.ComputeHash();

            // 4. 与上次打包时资源哈希值比较...
            bool needBuild = false;
            foreach (var c in categories)
            {
                if (c.ExistChangeItem)
                {
                    needBuild = true;
                    break;
                }
            }

            List<string> removedList;
            UpdateBuildingItemsFlag(categories, lastBuildConfig, out removedList);

            // 5. 打包
            if (needBuild)
            {
                yield return "打包中...";

                // 先打到一个临时目录
                UnityBuild(TempBuildingFolder, categories);

                // 检查是否所有的包都已正确生成
                bool succeed = AllAssetBundleBuildedSucceed(categories, TempBuildingFolder);
                if (!succeed)
                    throw new InvalidOperationException("生成失败，未知原因");

                // 将打的包移动到对应目录
                CopyFiles(TempBuildingFolder, BuildingRootFolder);

                yield return "打包完成！";

                // 打包完成事件
                foreach (var c in categories)
                {
                    if (c.ExistChangeItem)
                        c.OnBuildFinished();
                }
            }

            //
            foreach (var c in categories)
            {
                c.OnAfterAllBuildComplete();
            }

            StringBuilder sb = new StringBuilder();
            bool versionChanged = VersionChanged(categories, removedList, ref sb);

            string buildResult = sb.ToString();
            yield return buildResult;

            // 6. 生成列表文件，版本文件，日志文件
            yield return "生成版本控制文件...";
            Version = ReadVersionString();
            if (versionChanged)
            {
                SaveBuildingListFile(categories, ref sb);       // 供打包用的列表文件
                Version = VersionIncrease();
            }
            yield return "最新版本：" + Version;

            // 供下载用的列表文件
            int changedFileBytes;
            SaveLoadingListFile(categories, ref sb, out changedFileBytes);

            int kb = changedFileBytes / 1024;
            int mb = kb / 1024;
            int leftKB = kb % 1024;
            string sizeLog = string.Format("发生变化（新增或修改）的ab包大小：{0} MB {1} KB", mb, leftKB);
            yield return sizeLog;

            // list 文件信息
            SaveListInfoFile();

            // 日志
            if (versionChanged)
            {
                Log = sizeLog + "\n" + buildResult;
                SaveLog(sizeLog + "\n" + buildResult, Version);
            }

            // 删除移除的ab包
            DeleteRemovedAssetBundles(removedList);

            // dispose
            foreach (var c in categories)
                c.Dispose();
        }

        // 更新ab包对象的标志
        void UpdateBuildingItemsFlag(List<AssetCategory> categories, string[] lastBuildConfig, out List<string> removedList)
        {
            foreach (var c in categories)
            {
                foreach (var item in c.Items)
                {
                    if (string.IsNullOrEmpty(item.LastHash))
                        item.BuildingFlag = BuildingBundle.Flag.NewAdded;   // 新增的
                    else if (item.LastHash != item.CurrentHash)
                        item.BuildingFlag = BuildingBundle.Flag.Modified;   // 修改的
                    else
                        item.BuildingFlag = BuildingBundle.Flag.NoChange;
                }
            }

            removedList = new List<string>();
            if (lastBuildConfig != null)
            {
                foreach (var line in lastBuildConfig)
                {
                    bool removed = true;
                    foreach (var c in categories)
                    {
                        foreach (var item in c.Items)
                        {
                            if (item.RightConfig(line))
                            {
                                removed = false;
                                break;
                            }
                        }
                    }

                    // 删除的
                    if (removed)
                    {
                        string relativePath, hash;
                        BuildingBundle.ParseConfigLine(line, out relativePath, out hash);
                        removedList.Add(relativePath);
                    }
                }
            }

        }

        // 删除移除的ab包
        void DeleteRemovedAssetBundles(List<string> removedList)
        {
            for (int i = 0; i < removedList.Count; i++)
            {
                // 从磁盘中删除
                string fullPath = string.Format("{0}/{1}", BuildingRootFolder.TrimEnd('/'), removedList[i]);
                if (Directory.Exists(Path.GetDirectoryName(fullPath)))
                    File.Delete(fullPath);
            }
        }

        // 版本是否改变
        bool VersionChanged(List<AssetCategory> categories, List<string> removedList, ref StringBuilder log)
        {
            log.Length = 0;

            List<string> addedList = new List<string>();
            List<string> modifiedList = new List<string>();

            foreach (var c in categories)
            {
                foreach (var item in c.Items)
                {
                    if (item.BuildingFlag == BuildingBundle.Flag.NewAdded)                // 新增的
                        addedList.Add(item.OutputRelativePath);
                    else if (item.BuildingFlag == BuildingBundle.Flag.Modified)             // 修改的
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

        string ReadVersionString()
        {
            string path = VersionPath;
            if (File.Exists(path))
                return File.ReadAllText(path);
            return "0.0.0";
        }

        List<int> ReadVersion()
        {
            return ReadVersion(VersionPath);
        }

        string VersionIncrease()
        {
            return VersionIncrease(VersionPath);
        }

        string SetVersion(int version)
        {
            List<int> list = ReadVersion();
            int index = (int)BuildingConfig.CurrentProject;
            list[index] = version;
            string str = CombineVersionString(list);
            string savePath = VersionPath;
            File.WriteAllText(savePath, str);

            return str;
        }

        void SaveLoadingListFile(List<AssetCategory> categories, ref StringBuilder sb, out int changedABBytes)
        {
            sb.Length = 0;
            changedABBytes = 0;

            // calc hash
            for (int i = 0; i < categories.Count; i++)
            {
                for (int j = 0; j < categories[i].Items.Length; j++)
                {
                    var c = categories[i];
                    var item = c.Items[j];
                    string name = item.OutputRelativePath + item.Ext;
                    bool changed = item.BuildingFlag != BuildingBundle.Flag.NoChange;
                    string abPath = string.Format("{0}/{1}", BuildingRootFolder, name);
                    byte[] bytes = File.ReadAllBytes(abPath);
                    string hash = BuildingBundle.ComputeHash(bytes);
                    string strDownloadType = ReduceLocalAssetBundleTool.GetLoginDownloadString(name);

                    sb.AppendFormat("{0},{1},{2},{3};", name, hash, bytes.Length, strDownloadType);

                    if (changed)
                        changedABBytes += bytes.Length;
                }
            }

            File.WriteAllText(LoadingListPath, sb.ToString());
        }

        void SaveBuildingListFile(List<AssetCategory> categories, ref StringBuilder sb)
        {
            sb.Length = 0;
            foreach (var c in categories)
            {
                foreach (var item in c.Items)
                {
                    string config = item.GenerateConfigLine();
                    sb.AppendLine(config);
                }
            }

            FolderHelper.CreateFileFolder(BuildingListPath);
            File.WriteAllText(BuildingListPath, sb.ToString());
        }

        void SaveLog(string log, string version)
        {
            if (string.IsNullOrEmpty(log))
                return;

            string oldLog = "";
            string path = BuildingLogPath;
            if (File.Exists(path))
                oldLog = File.ReadAllText(path);
            string currentLog = string.Format("打包时间：{0}\n项目：{4}\n版本：{1}\n{2}\n--------------------------------\n\n{3}", DateTime.Now, version, log, oldLog, BuildingConfig.CurrentProject);
            FolderHelper.CreateFileFolder(path);
            File.WriteAllText(path, currentLog);
        }

        void SaveListInfoFile()
        {
            SaveListInfoFile(BuildingRootFolder);
        }

    }
}
