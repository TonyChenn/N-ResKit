using HybridCLR.Editor;
using NDebug;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class Task_CompileDLL
{
	/// <summary>
	/// 编译dll ,出自HybridCLR.CompileDllHelper.CompileDll
	/// </summary>
	[MenuItem("HybridCLR/CompileAndCopyDll")]
	public static void BuildDllBundle()
	{
		CompileAndCopyDll(false);
	}


	public static void CompileAndCopyDll(bool copyAotDll = true)
	{
		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
		HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();

		string desFolder = Application.dataPath + "/BuildBundle/Code/" + target;
		if (Directory.Exists(desFolder)) Directory.Delete(desFolder, true);
		Directory.CreateDirectory(desFolder);

		List<string> failList = new List<string>();
		CopyHotfixDll(desFolder, ref failList);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		if (failList.Count > 0) return;
		failList.Clear();

		if (copyAotDll)
		{
			failList.Clear();
			CopyAOTDlls(ref failList);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}
		if (failList.Count > 0) return;

		Debug.LogFormat($"编译程序集成功");
	}

	private static void CopyHotfixDll(string outputDir, ref List<string> failList)
	{
		BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
		string hotfixDllDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
		foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
		{
			string dllSrcPath = Path.Combine(hotfixDllDir, dll);
			string dllDestPath = $"{outputDir}/{dll[..dll.LastIndexOf(".")]}.bytes";
			if (File.Exists(dllSrcPath))
			{
				File.Copy(dllSrcPath, dllDestPath, true);
			}
			else
			{
				Log.Error($"热更程序集不存在：{dllSrcPath}");
				failList.Add(dllSrcPath);
			}
		}
	}

	private static void CopyAOTDlls(ref List<string> failList)
	{
		string aotSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget);
		string aotDesDir = Application.dataPath + "/Resources/AotDll";
		if (Directory.Exists(aotDesDir)) { Directory.Delete(aotDesDir, true); }
		Directory.CreateDirectory(aotDesDir);

		foreach (var dll in SettingsUtil.AOTAssemblyNames)
		{
			string dllPath = $"{aotSrcDir}/{dll}.dll";
			if (!File.Exists(dllPath))
			{
				failList.Add(dllPath);
				Log.Error($"AOT程序集不存在：{dllPath}");
				continue;
			}
			string targetPath = $"{aotDesDir}/{dll}.bytes";
			File.Copy(dllPath, targetPath, true);
		}
		Debug.Log("完成拷贝AOT程序集");
	}
}
