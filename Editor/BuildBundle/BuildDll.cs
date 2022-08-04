using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace AssetBundle
{
    public static class BuildDll
    {
        /// <summary>
        /// 编译dll ,出自HybridCLR.CompileDllHelper.CompileDll
        /// </summary>
        public static void CompileDll(BuildTarget target)
        {
            // 生成dll目录
            string buildDir = $"{Application.dataPath}/../HybridCLRData/HotFixDlls/{target}";
            // 拷贝到Assets下，修改为bytes拓展名
            string dllBytesDir = $"{Application.dataPath}/BuildBundle/Code/{target}";
            if (!Directory.Exists(dllBytesDir))
                Directory.CreateDirectory(dllBytesDir);

            var group = BuildPipeline.GetBuildTargetGroup(target);
            ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
            scriptCompilationSettings.group = group;
            scriptCompilationSettings.target = target;
            Directory.CreateDirectory(buildDir);
            ScriptCompilationResult scriptCompilationResult =
                PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
            foreach (var assemby in scriptCompilationResult.assemblies)
            {
                File.Copy($"{buildDir}/{assemby}", $"{dllBytesDir}/{assemby}.bytes", true);
            }
            Debug.Log("完成编译程序集");

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
                string dllBytesPath = $"{dllBytesDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
            }
            Debug.Log("完成拷贝AOT程序集");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            Debug.Log("编译程序集完成");
        }

        public static void BuildDllBundle(BuildTarget target)
        {
            string dllFolder = $"{Application.dataPath}/BuildBundle/Code/{target}";
            List<string> dlls = Directory.GetFiles(dllFolder, "*.dll.bytes").ToList<string>();


            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = "code.u",
                assetNames = dlls.Select(s => getFileName(s)).ToArray(),
            };
            string outputFolder = BuildBundlePrefs.BuildBundleFolderPath;
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
            BuildPipeline.BuildAssetBundles(outputFolder,
                new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, target);

            Debug.Log("打包dll完成");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        private static string getFileName(string filePath)
        {
            return filePath.Substring(filePath.IndexOf("Assets/"));
        }
    }
}

