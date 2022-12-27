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
            EditorUtility.DisplayDialog("����", "û��Ҫ�����������Ϣ", "�õ�");
            yield break;
        }

        // ��ȡ�ϴδ��������Ϣ
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
            yield return "���ǵ�һ�δ����";
        }
        // ����Hash֮ǰ
        categories.ForEach((asset) => { asset.OnBeforeComputeHash(); });

        // ����Hash
        yield return "���ڼ�����Դ�Ĺ�ϣֵ...";
        categories.ForEach((asset) => { asset.ComputeHash(); });

        // ���ϴδ���Աȣ��Ƿ�����Դ�仯
        bool needBuild = false;
        categories.ForEach((asset) => { if (asset.HasChangedItem) { needBuild = true;} });

        // ������Ҫɾ������Դ
        List<string> removedList;
        // ˢ�´�����
        foreach (var c in categories)
        {
            foreach (var item in c.Bundles)
            {
                if (string.IsNullOrEmpty(item.LastHash)) item.BuildingFlag = BaseBundle.Flag.NewAdded;
                else if (item.LastHash == item.CurrentHash) item.BuildingFlag = BaseBundle.Flag.NoChange;
                else item.BuildingFlag |= BaseBundle.Flag.Modified;
            }
        }
        // TODO �����Ҫ�Ƴ�����Դ


        if (needBuild)
        {
            // �������ʱĿ¼
            yield return "���ڴ��...";
            buildHandler(TmpFolder, categories);

            // �����û�д������Դ
            bool sucess = allBuildSuccess(categories);
            if (!sucess) throw new InvalidOperationException("���ʧ�ܣ�δ֪ԭ��");

            // �ƶ�����ʽĿ¼
            CopyBundles(TmpFolder, BuildRootFolder);
            yield return "������";

            // �������¼�
            categories.ForEach((asset) => { asset.OnBuildFinished(); });
        }
        // ��������¼�
        categories.ForEach((asset) => { asset.OnAllBuildCompleted(); });

        //TODO ���ɰ��
        yield return "���ɰ���ļ�...";

        // �����ȸ��ļ�
        int changedSize = createHashFile(categories);
        int kb = changedSize / 1024;
        int mb = kb / 1024;
        kb = changedSize % 1024;
        yield return $"ab�����´�С�仯��{mb}MB, {kb}KB";

        createVersionFile();

        // TODO ɾ��Ҫ�Ƴ�����Դ

        // dispose
        categories.ForEach((asset) => { asset.Dispose(); });
    }

    /// <summary>
    /// ������Դ�ı�־
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

        // ׼�����
        List<AssetBundleBuild> list = new List<AssetBundleBuild>(128);
        foreach (var item in categories)
        {
            if (item.HasChangedItem)
            {
                // ��������Դ
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


    // ��������ļ�
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

    // ������Դ�б�
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
