using HybridCLR.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

/// <summary>
/// aot 和 hotupdate 的dll不打的方式放进目标目录
/// </summary>
public static class BuildDll
{
    /// <summary>
    /// 编译dll ,出自HybridCLR.CompileDllHelper.CompileDll
    /// </summary>
    [MenuItem("HybridCLR/BuildDll")]
    public static void BuildDllBundle()
    {
        CompileAndBuildDll(Application.streamingAssetsPath);
    }


    public static void CompileAndBuildDll(string defaultOutputFolder = null)
    {
        if (string.IsNullOrEmpty(defaultOutputFolder)) defaultOutputFolder = Path_BuildBundle.BuildBundleFolderPath;

        // 输出目录
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        string outputDir = $"{defaultOutputFolder}/dll";
        if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

        var buildDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        var group = BuildPipeline.GetBuildTargetGroup(target);

        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
        scriptCompilationSettings.group = group;
        scriptCompilationSettings.target = target;
        Directory.CreateDirectory(buildDir);
        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
        foreach (var assemby in scriptCompilationResult.assemblies)
        {
            Debug.LogFormat($"编译程序集:{buildDir}/{assemby} 成功");
            File.Copy($"{buildDir}/{assemby}", $"{outputDir}/{assemby.Replace(".dll", ".u").ToLower()}", true);
        }
        Debug.Log("编译程序集完成，并复制到输出目录：" + outputDir);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);


        // AOT补充元数据dll--->outputDir
        CopyAOTAssembliesToOutputDir(outputDir);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private static void CopyAOTAssembliesToOutputDir(string outputDir)
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);

        if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
            if (!File.Exists(srcDllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }
            string dllBytesPath = $"{outputDir}/{dll.ToLower()}.u";
            File.Copy(srcDllPath, dllBytesPath, true);
        }

        Debug.Log("完成拷贝AOT程序集");
    }
}
