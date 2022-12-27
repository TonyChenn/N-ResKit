using HybridCLR.Editor;
using NCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class BuildAB
{
    private static string m_targetFolder;
    private static string m_tempFolder;
    private static string BuildRootFolder
    {
        get { return m_targetFolder; }
        set { m_targetFolder = value.Replace("\\", "/").TrimEnd('/'); }
    }

    private static string TmpFolder
    {
        get
        {
            if (m_tempFolder == null)
                m_tempFolder = Path.GetFullPath(Application.dataPath + "/../temp_build");
            return m_tempFolder;
        }
    }

    private static string TmpHashFile { get { return $"{TmpFolder}/main.csv"; } }
    private static string HashFile { get { return $"{BuildRootFolder}/main.csv"; } }

    public static IEnumerator Build(string targetFolder, bool rebuildAll)
    {
        BuildRootFolder = targetFolder;

        List<AssetCategory> categories = BuildingConfig.GetConfig();
        if (categories == null || categories.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "没有要打包的配置信息", "好的");
            yield break;
        }

        // 读取上次打包配置信息
        string[] lash_build_config = null;
        if (File.Exists(HashFile))
        {
            lash_build_config = File.ReadAllLines(HashFile);
            Dictionary<string, string[]> cfg = new Dictionary<string, string[]>();
            for (int i = 0, iMax = lash_build_config.Length; i < iMax; i++)
            {
                string[] item = lash_build_config[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                cfg[item[0]] = item;
            }

            foreach (var c in categories)
            {
                foreach (var item in c.Bundles)
                {
                    if (cfg.ContainsKey(item.AssetBundleName))
                    {
                        item.LastHash = cfg[item.AssetBundleName][1];
                    }
                }
            }
        }
        else
        {
            yield return "这是第一次打包。";
        }
        // 计算Hash之前
        categories.ForEach((asset) => { asset.OnBeforeComputeHash(); });

        // 计算Hash
        yield return "正在计算资源的哈希值...";
        categories.ForEach((asset) => { asset.ComputeHash(); });

        // 和上次打包对比，是否有资源变化
        bool needBuild = false;
        categories.ForEach((asset) => { if (asset.HasChangedItem) { needBuild = true;} });

        // 计算需要删除的资源
        List<string> removedList;
        // 刷新打包标记
        foreach (var c in categories)
        {
            foreach (var item in c.Bundles)
            {
                if (string.IsNullOrEmpty(item.LastHash)) item.BuildingFlag = BaseBundle.Flag.NewAdded;
                else if (item.LastHash == item.CurrentHash) item.BuildingFlag = BaseBundle.Flag.NoChange;
                else item.BuildingFlag |= BaseBundle.Flag.Modified;
            }
        }
        // TODO 标记需要移除的资源


        if (needBuild)
        {
            // 打包到临时目录
            yield return "正在打包...";
            buildHandler(TmpFolder, categories);

            // 检查有没有打包坏资源
            bool sucess = allBuildSuccess(categories);
            if (!sucess) throw new InvalidOperationException("打包失败，未知原因");

            // 移动到正式目录
            CopyBundles(TmpFolder, BuildRootFolder);
            yield return "打包完成";

            // 打包完成事件
            categories.ForEach((asset) => { asset.OnBuildFinished(); });
        }
        // 打包结束事件
        categories.ForEach((asset) => { asset.OnAllBuildCompleted(); });

        //TODO 生成版控
        yield return "生成版控文件...";

        // 生成热更文件
        int changedSize = createHashFile(categories);
        int kb = changedSize / 1024;
        int mb = kb / 1024;
        kb = changedSize % 1024;
        yield return $"ab包更新大小变化，{mb}MB, {kb}KB";

        createVersionFile();

        // TODO 删除要移除的资源

        // dispose
        categories.ForEach((asset) => { asset.Dispose(); });
    }

    /// <summary>
    /// 更新资源的标志
    /// </summary>
    private static void updateBundleFlag(List<AssetCategory> categories, out List<string> removedList)
    {
        foreach (AssetCategory category in categories)
        {
            foreach (var item in category.Bundles)
            {
                if (string.IsNullOrEmpty(item.LastHash))
                    item.BuildingFlag = BaseBundle.Flag.NewAdded;
                else if (item.LastHash != item.CurrentHash)
                    item.BuildingFlag = BaseBundle.Flag.Modified;
                else
                    item.BuildingFlag = BaseBundle.Flag.NoChange;
            }
        }

        removedList = new List<string>();


    }

    private static void buildHandler(string folder, List<AssetCategory> categories)
    {
        if (Directory.Exists(folder)) Directory.Delete(folder, true);
        Directory.CreateDirectory(folder);

        // 准备打包
        List<AssetBundleBuild> list = new List<AssetBundleBuild>(128);
        foreach (var item in categories)
        {
            if (item.HasChangedItem)
            {
                // 无需打包资源
                if (item is UnBuildAssets)
                {
                    (item as UnBuildAssets).Build(folder);
                }
                else
                {
                    item.PrepareBuild();
                    list.AddRange(item.AssetBundleBuilds);
                }
            }
        }

        AssetDatabase.Refresh();

        // build
        BuildPipeline.BuildAssetBundles(folder, list.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        // del manifest
        string[] manifests = Directory.GetFiles(folder, "*.manifest", SearchOption.AllDirectories);
        foreach (var item in manifests)
        {
            File.Delete(item);
        }
        string folderManifest = $"{folder}/{Path.GetFileNameWithoutExtension(folder)}";
        File.Delete(folderManifest);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private static bool allBuildSuccess(List<AssetCategory> categories)
    {
        bool success = true;

        foreach (var c in categories)
        {
            foreach (var item in c.Bundles)
            {
                if (item.BuildingFlag != BaseBundle.Flag.NoChange)
                {
                    if (!File.Exists($"{TmpFolder}/{item.AssetBundleName}"))
                    {
                        success = false;
                        break;
                    }
                }
            }
        }

        return success;
    }

    private static void CopyBundles(string fromFolder, string toFolder)
    {
        string[] files = Directory.GetFiles(fromFolder, "*.*", SearchOption.AllDirectories);
        for (int i = 0, iMax = files.Length; i < iMax; i++)
        {
            string relatice_path = files[i].Substring(fromFolder.Length);
            string path = toFolder + relatice_path;

            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            File.Copy(files[i], path, true);
        }
    }


    // 创建版控文件
    private static void createVersionFile()
    {
        string versionFile = $"{TmpFolder}/version.dat";
        if (!File.Exists(versionFile))
            File.WriteAllText(versionFile, "{\"version\":\"0.0.0\",\"cdn_url\":\"www.baidu.com\", \"time\":0}");

        string json = File.ReadAllText(versionFile);
        JSONNode node = JSON.Parse(json);
        string cur_version = node["version"];
        string[] strs = cur_version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        int version = int.Parse(strs[2]);

        node["version"] = $"{strs[0]}.{strs[1]}.{version + 1}";
        node["time"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        File.WriteAllText(versionFile, node.ToString());
    }

    // 生成资源列表
    private static int createHashFile(List<AssetCategory> categories)
    {
        if (File.Exists(HashFile)) File.Create(HashFile).Dispose();

        StringBuilder builder = new StringBuilder(512);
        int result = 0;
        foreach (var c in categories)
        {
            foreach (var item in c.Bundles)
            {
                bool changed = item.BuildingFlag != BaseBundle.Flag.NoChange;
                string path = $"{BuildRootFolder}/{item.AssetBundleName}";
                byte[] bytes = File.ReadAllBytes(path);
                string hash = MD5Helper.ComputeHash(bytes);

                builder.AppendLine($"{item.AssetBundleName},{item.CurrentHash},{bytes.Length}");

                if (changed) { result += bytes.Length; }
            }
        }
        File.WriteAllText(HashFile, builder.ToString());

        return result;
    }
}
