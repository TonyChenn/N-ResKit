using HybridCLR.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

public static class BuildDll
{
    /// <summary>
    /// 编译dll ,出自HybridCLR.CompileDllHelper.CompileDll
    /// </summary>
    public static void CompileDll(BuildTarget target)
    {
        // 生成dll目录
        string buildDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        // 拷贝到Assets下，修改为bytes拓展名
        string outputDir = $"{Application.dataPath}/BuildBundle/DLL/{target}";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        var group = BuildPipeline.GetBuildTargetGroup(target);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
        scriptCompilationSettings.options = ScriptCompilationOptions.DevelopmentBuild;
        scriptCompilationSettings.group = group;
        scriptCompilationSettings.target = target;
        Directory.CreateDirectory(buildDir);
        ScriptCompilationResult scriptCompilationResult =
            PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
        foreach (var assemby in scriptCompilationResult.assemblies)
        {
            File.Copy($"{Application.dataPath}{buildDir}/{assemby}", $"{outputDir}/{assemby}.bytes", true);
        }
        Debug.Log($"完成编译程序集,已复制到Assets/BuildBundle/DLL{target}");

        // 补充AOT dll
        string aotDllDir = $"{Application.dataPath}/../HybridCLRData/AssembliesPostIl2CppStrip/{target}";
        string[] AOTMetaDlls = new string[]
        {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll",
        };
        foreach (var dll in AOTMetaDlls)
        {
            string dllPath = $"{aotDllDir}/{dll}";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }
            File.Copy(dllPath, $"{outputDir}/{dll}.bytes", true);
        }
        Debug.Log("完成拷贝AOT程序集");

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        Debug.Log("编译程序集完成");
    }

    public static void BuildDllBundle(BuildTarget target)
    {
        string dllFolder = $"{Application.dataPath}/BuildBundle/DLL/{target}";
        List<string> dlls = Directory.GetFiles(dllFolder, "*.dll.bytes").ToList<string>();

        List<AssetBundleBuild> bundles = new List<AssetBundleBuild>(16);
        for (int i = 0; i < dlls.Count; i++)
        {
            FileInfo info = new FileInfo(dlls[i]);
            bundles.Add(new AssetBundleBuild
            {
                assetBundleName = "dll/" + info.Name.ToLower().Replace("dll.bytes", "u"),
                assetNames = new string[] { dlls[i].Substring(dlls[i].IndexOf("Assets/")) },
            });
        };
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        string outputFolder = Path_BuildBundle.BuildBundleFolderPath;
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        BuildPipeline.BuildAssetBundles(outputFolder, bundles.ToArray(),
                                        BuildAssetBundleOptions.None, target);

        // delete manifest files
        string[] manifests = Directory.GetFiles(outputFolder, "*.manifest", SearchOption.AllDirectories);
        string[] metas = Directory.GetFiles(outputFolder, "*.manifest.meta", SearchOption.AllDirectories);
        for (int i = 0; i < manifests.Length; i++)
            File.Delete(manifests[i]);
        for (int i = 0; i < metas.Length; i++)
            File.Delete(metas[i]);
        var folderFile = $"{outputFolder}/{Path.GetFileNameWithoutExtension(outputFolder)}";
        var folderMeta = $"{folderFile}.meta";
        File.Delete(folderFile);
        File.Delete(folderMeta);

        Debug.Log("打包dll完成");
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}

