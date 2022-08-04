using AssetBundle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


public class ReduceLocalAssetBundleTool : ScriptableObject
{
    const string LocalFolder = "Assets/StreamingAssets";
    const string TempFolder = "AssetBundles/TempForReduse";


    // 安卓小包
    static string[] PersistFoldersForAndroid = new string[]
    { 
        /*
        "effect/scene",
        "effect/ui",
        "localize",
        "txt",
        "ui",
        "image/itemicon",*/

        // 以下是必需的
        "audio/androideffect",
        "localize/text",
        "data",
    };

    static string[] PertsistFilesForAndroid = new string[] 
    {
        // 以下是必需的
        "model/stage/dabai_loading.u",
        "model/stage/uifx_denglu.u",
        "model/stage/xuanrenjiemian.u",
        "image/ui_bg/notice_bg_title.u",
        "image/ui_bg/notice_bg.u",
        "image/ui_bg/bg_loading_1.u",
        "image/ui_bg/bg_loading_2.u",
        "image/ui_bg/bg_loading_3.u",
        "effect/ui/uifx_touch.u",
        "ui/ui_message.u",
        "ui/atlas_common.u",
        "ui/common.u",
        "ui/fzcuqian.u",
        "ui/ui_login.u",
        "ui/ui_world.u",
        "lua/luascripts.u",
        "txt/sensitivewords.u",
        "atlases/particlesystematlas.u",
        "audio/effect/click.u",
        "audio/music/19999ywty.u",
    };

    // Ios 小包
    static string[] PersistFoldersForIos = new string[]
    { 
        "animation",
        "assetdata",
        "atlases",

        //"audio/androideffect",
        //"audio/effect",

        "effect/scene",
        "effect/ui",

        "image",
        "localize",
        "lua",

        "model/baby",
        "model/character",
        "model/material",
        "model/other",
        "model/stage",

        "scene",
        "timeline",
        "txt",
        "ui",

        "audio",

        // 以下是必需的
        "audio/androideffect",
        "localize/text",
        "data",
    };

    static string[] PersistFilesForIos = new string[]
    {
       "model/cloth/hair_10025.u",
       "model/cloth/hair_10026.u",
       "model/cloth/hair_50025.u",
       "model/cloth/hair_50026.u",
       "model/cloth/coat_13025.u",
       "model/cloth/coat_13026.u",
       "model/cloth/coat_53025.u",
       "model/cloth/coat_53026.u",
       "model/cloth/pants_16025.u",
       "model/cloth/pants_16026.u",
       "model/cloth/pants_56025.u",
       "model/cloth/pants_56026.u",
       "model/cloth/shoes_19025.u",
       "model/cloth/shoes_19026.u",
       "model/cloth/shoes_59025.u",
       "model/cloth/shoes_59026.u",
       "model/cloth/face_32004.u",
       "model/cloth/face_32000.u",
       "model/cloth/face_72000.u",
       "model/cloth/face_72009.u",
       "model/cloth/gloves_40000.u",
       "model/cloth/gloves_80000.u",
       "audio/music/19999ywty.u",

        // 以下是必需的
        "model/stage/dabai_loading.u",
        "model/stage/uifx_denglu.u",
        "model/stage/xuanrenjiemian.u",
        "image/ui_bg/notice_bg_title.u",
        "image/ui_bg/notice_bg.u",
        "image/ui_bg/bg_loading_1.u",
        "image/ui_bg/bg_loading_2.u",
        "image/ui_bg/bg_loading_3.u",
        "effect/ui/uifx_touch.u",
        "ui/ui_message.u",
        "ui/atlas_common.u",
        "ui/common.u",
        "ui/fzcuqian.u",
        "ui/ui_login.u",
        "ui/ui_world.u",
        "lua/luascripts.u",
        "txt/sensitivewords.u",
        "atlases/particlesystematlas.u",
        "audio/effect/click.u",
        "audio/music/19999ywty.u",
    };

    // 登陆时下载
    static string[] LoginDownloadFolders = new string[]
    { 
        "animation",
        "assetdata",
        "atlases",
        "data",
        "audio",
        "effect/scene",
        "effect/ui",

        "image",
        "localize",
        "lua",

        "model/baby",
        "model/character",
        "model/material",
        "model/other",
        "model/stage",

        "scene",
        "timeline",
        "txt",
        "ui",
    };

    static string[] LoginDownloadFiles = new string[]
    {
       "model/cloth/hair_10025.u",
       "model/cloth/hair_10026.u",
       "model/cloth/hair_50025.u",
       "model/cloth/hair_50026.u",
       "model/cloth/coat_13025.u",
       "model/cloth/coat_13026.u",
       "model/cloth/coat_53025.u",
       "model/cloth/coat_53026.u",
       "model/cloth/pants_16025.u",
       "model/cloth/pants_16026.u",
       "model/cloth/pants_56025.u",
       "model/cloth/pants_56026.u",
       "model/cloth/shoes_19025.u",
       "model/cloth/shoes_19026.u",
       "model/cloth/shoes_59025.u",
       "model/cloth/shoes_59026.u",
       "model/cloth/face_32004.u",
       "model/cloth/face_32000.u",
       "model/cloth/face_72000.u",
       "model/cloth/face_72009.u",
       "model/cloth/gloves_40000.u",
       "model/cloth/gloves_80000.u",
       "audio/music/19999ywty.u",

        // 以下是必需的
        "model/stage/dabai_loading.u",
        "model/stage/uifx_denglu.u",
        "model/stage/xuanrenjiemian.u",
        "image/ui_bg/notice_bg_title.u",
        "image/ui_bg/notice_bg.u",
        "image/ui_bg/bg_loading_1.u",
        "image/ui_bg/bg_loading_2.u",
        "image/ui_bg/bg_loading_3.u",
        "effect/ui/uifx_touch.u",
        "ui/ui_message.u",
        "ui/atlas_common.u",
        "ui/common.u",
        "ui/fzcuqian.u",
        "ui/ui_login.u",
        "ui/ui_world.u",
        "lua/luascripts.u",
        "txt/sensitivewords.u",
        "atlases/particlesystematlas.u",
        "audio/effect/click.u",
        "audio/music/19999ywty.u",
    };


    [MenuItem("BuildTool/AssetBundle/Reduse Local For Android")]
    static void ReduseLocalForAndroid()
    {
        ReduseLocal(PersistFoldersForAndroid, PertsistFilesForAndroid);
    }

    [MenuItem("BuildTool/AssetBundle/Reduse Local For Ios")]
    static void ReduseLocalForIos()
    {
        ReduseLocal(PersistFoldersForIos, PersistFilesForIos);
    }

    static void ReduseLocal(string[] persistFolders, string[] pertsistFiles)
    {
        if (Directory.Exists(TempFolder))
        {
            Debug.Log("Failed! Use reverse reduce first please.");
            return;
        }

        List<string> persistFiles = new List<string>();

        // copy .meta 文件
        string[] metaFiles = Directory.GetFiles(LocalFolder, "*.meta", SearchOption.AllDirectories);
        foreach (var f in metaFiles)
        {
            string relativePath = f.Replace("\\", "/").Substring(LocalFolder.Length).Trim('/');
            string newPath = Path.Combine(TempFolder, relativePath);
            FolderHelper.CreateFileFolder(newPath);
            File.Copy(f, newPath);
        }

        // move 文件
        string[] files = Directory.GetFiles(LocalFolder, "*.*", SearchOption.AllDirectories);
        foreach (var f in files)
        {
            string fileName = Path.GetFileName(f);
            string ext = Path.GetExtension(f);
            if (ext == ".meta" || fileName == "package_config.json" || fileName == ".git")
                continue;

            string relativePath = f.Replace("\\", "/").Substring(LocalFolder.Length).Trim('/');
            bool persist = ext == ".dat"
                || Array.FindIndex(persistFolders, s => relativePath.StartsWith(s + "/")) > -1
                || Array.FindIndex(pertsistFiles, s => relativePath == s) > -1;
            if (persist)
            {
                if (!persistFiles.Contains(relativePath))
                    persistFiles.Add(relativePath);
            }
            else
            {
                string newPath = Path.Combine(TempFolder, relativePath);
                FolderHelper.CreateFileFolder(newPath);
                File.Move(f, newPath);
            }
        }

        // 删除空文件夹
        string[] dirs = Directory.GetDirectories(LocalFolder, "*", SearchOption.AllDirectories);
        foreach (var dir in dirs)
        {
            if (Directory.Exists(dir))
            {
                string[] filesInDir = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                if (filesInDir.Count(s => Path.GetExtension(s) != ".meta") < 1)
                    Directory.Delete(dir, true);
            }
        }

        // copy version file
        StringBuilder sb = new StringBuilder();
        string[] versionFiles = Directory.GetFiles(LocalFolder, "*.dat", SearchOption.TopDirectoryOnly);
        foreach (var vf in versionFiles)
        {
            string fileName = Path.GetFileName(vf);
            string newPath = Path.Combine(TempFolder, fileName);
            FolderHelper.CreateFileFolder(newPath);
            File.Copy(vf, newPath);

            // fix version
            sb.Length = 0;
            if (fileName == "version.dat")
            {
                sb.AppendLine("0.0.0");
            }
            else
            {
                string text = File.ReadAllText(vf);
                string[] lines = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (persistFiles.Any(pf => line.StartsWith(pf)))
                        sb.Append(line + ";");
                }
            }

            File.Delete(vf);
            File.WriteAllText(vf, sb.ToString());
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        Debug.Log("Reduce Done!");
    }

    [MenuItem("BuildTool/AssetBundle/Reverse Reduse Local")]
    static void RevertReduseLocal()
    {
        if (!Directory.Exists(TempFolder))
        {
            Debug.Log("Failed!");
            return;
        }

        // move 文件
        string[] files = Directory.GetFiles(TempFolder, "*.*", SearchOption.AllDirectories);
        foreach (var f in files)
        {
            string relativePath = f.Replace("\\", "/").Substring(TempFolder.Length).Trim('/');
            string newPath = Path.Combine(LocalFolder, relativePath);
            FolderHelper.CreateFileFolder(newPath);
            File.Delete(newPath);
            File.Move(f, newPath);
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        Directory.Delete(TempFolder, true);

        Debug.Log("Reverse Reduce Done!");
    }

    /// <summary>
    /// Jenkins使用
    /// </summary>
    static void ReduseLocalAssets()
    {
        ReduseLocalForAndroid();
    }

    /// <summary>
    /// Jenkins使用
    /// </summary>
    static void RevertReduseLocalAssets()
    {
        RevertReduseLocal();
    }

    // 应用动态加载的配置
    [MenuItem("BuildTool/AssetBundle/Apply dynamic load config")]
    public static void ApplyDynamicLoadConfig()
    {
        string folder = Application.streamingAssetsPath;
        Debug.Log("Bundle folder: " + folder);
        string[] projects = Enum.GetNames(typeof(BuildingConfig.Project));

        foreach (var item in projects)
        {
            string fileName = item.ToString().ToLower();
            string path = string.Format("{0}/{1}.dat", folder, fileName);
            ApplyDynamicLoadConfig(path);
        }
    }

    static void ApplyDynamicLoadConfig(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("File not exits, path: " + path);
            return;
        }

        string text = File.ReadAllText(path);
        string[] lines = text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();

        foreach (var line in lines)
        {
            string[] words = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string name = words[0];
            string hash = words[1];
            string size = words[2];
            string str = GetLoginDownloadString(name);

            sb.AppendFormat("{0},{1},{2},{3};", name, hash, size, str);
        }

        File.WriteAllText(path, sb.ToString());
        Debug.Log(string.Format("Apply dynamic config done: {0}", path));
    }

    public static string GetLoginDownloadString(string name)
    {
        bool contains = Array.FindIndex(LoginDownloadFolders, s => name.StartsWith(s + "/")) > -1
                    || Array.FindIndex(LoginDownloadFiles, s => name == s) > -1;
        string str = contains ? "1" : "0";
        return str;
    }


}
