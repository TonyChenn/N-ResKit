using AssetBundle;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
public static class ProjectBuilder
{
    private static string fromPath;
    private static string toPath;
    #region Decode arguments from shell

    static string GetArg(string name, string defaultArg)
    {
        if (string.IsNullOrEmpty(name))
            return defaultArg;

        string[] args = Environment.GetCommandLineArgs();
        string argStart = string.Format("{0}=", name);

        foreach (string arg in args)
        {
            if (arg.StartsWith(argStart))
            {
                string argFixed = arg.Substring(argStart.Length);
                return string.IsNullOrEmpty(argFixed) ? defaultArg : argFixed;
            }
        }

        return defaultArg;
    }

    static bool GetArg(string name, bool defaultArg)
    {
        string arg = GetArg(name, null);
        if (string.IsNullOrEmpty(arg))
            return defaultArg;

        bool b;
        if (bool.TryParse(arg, out b))
            return b;

        return defaultArg;
    }

    #endregion

    public static void BuildAssetBundle()
    {
        BuildArguments arg = new BuildArguments();
        arg.BuildProject = GetArg("Build", arg.BuildProject);
        arg.ConvertTable = GetArg("ConvertTable", arg.ConvertTable);
        arg.UploadToFTP = GetArg("ManualUpload", arg.UploadToFTP);
        arg.BuildAll = GetArg("RebuildAll", arg.BuildAll);

        BuildWindow.Create().Build();
        
    }
    /// <summary>
    /// 复制要上传到ftp的文件到jenkins的工作路径
    /// </summary>
    /// <param name="formPath"></param>
    /// <param name="toPath"></param>
    /// <returns></returns>
    static bool CopyFilesToJenkins(string formPath, string toPath, string fileType)
    {

        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }
        #region //将对比文件夹下的文件复制到jenkins工作目录
        try
        {
            string[] allFolders = Directory.GetDirectories(formPath);//文件夹
            string[] allFiles = Directory.GetFiles(formPath);//文件
            if (allFiles.Length > 0)
            {
                for (int i = 0; i < allFiles.Length; i++)
                {

                    File.Copy(formPath + fileType + Path.GetFileName(allFiles[i]), toPath + fileType + Path.GetFileName(allFiles[i]), true);
                }
            }
            if (allFolders.Length > 0)
            {
                for (int j = 0; j < allFolders.Length; j++)
                {
                    Directory.GetDirectories(formPath + fileType + Path.GetFileName(allFolders[j]));

                    //递归调用
                    CopyFilesToJenkins(formPath + fileType + Path.GetFileName(allFolders[j]), toPath + fileType + Path.GetFileName(allFolders[j]), fileType);
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
        #endregion
        return true;
    }
    public static void AndroidCopyFolder()
    {
        fromPath = GetArg("FROM_PATH", toPath);
        toPath = GetArg("TO_PATH", toPath);
        string fileType = "\\";
        //Debug.Log(fromPath);
        //Debug.Log(toPath);
        CopyFilesToJenkins(fromPath, toPath, fileType);
    }
    public static void IOSCopyFolder()
    {
        fromPath = GetArg("FROM_PATH", toPath);
        toPath = GetArg("TO_PATH", toPath);
        string fileType = "/";
        //Debug.Log(fromPath);
        //Debug.Log(toPath);
        CopyFilesToJenkins(fromPath, toPath, fileType);
    }
    public static void PrepareBuild()
    {
        AssetDatabase.Refresh();
    }

    public static void BuildPlayerForAndroid()
    {
        // build
        string path = GetArg("Path", "");
        BuildTarget target = BuildTarget.Android;
        BuildOptions options = BuildOptions.None;

        BuildPipeline.BuildPlayer(BuildingScenes, path, target, options);
    }

    public static void BuildPlayerForIOS()
    {
        DateTime now = DateTime.Now;
        string path = GetArg("Path", "");
        BuildTarget target = BuildTarget.iOS;
        BuildOptions options = BuildOptions.None;

        //UnityEngine.Debug.Log (string.Format("path: {0}", path));

        BuildPipeline.BuildPlayer(BuildingScenes, path, target, options);
    }

    [PostProcessBuild(0)]
    static void OnPostProcessBuild(BuildTarget target, string path)
    {
        UnityEngine.Debug.Log(string.Format("Build finished, on post process build, target: {0}, path: {1}", target, path));
    }

    static string[] BuildingScenes
    {
        get
        {
            List<string> scenes = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene != null && scene.enabled)
                    scenes.Add(scene.path);
            }
            return scenes.ToArray();
        }
    }
}
