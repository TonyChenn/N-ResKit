using HybridCLR.Editor;
using NDebug;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class Task_CompileDLL
{
	private static readonly string HOTFIX_DLL_FOLDER = Application.dataPath + "/BuildBundle/Code";
	private static readonly string AOT_DLL_FOLDER = Application.dataPath + "/Resources/AotDll";
	/// <summary>
	/// 编译dll ,出自HybridCLR.CompileDllHelper.CompileDll
	/// </summary>
	[MenuItem("HybridCLR/CompileAndCopyDll")]
	static void BuildDllBundle()
	{
		CompileAndCopyDll(false);
	}


	public static void CompileAndCopyDll(bool copyAotDll = true)
	{
		HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
		Log.GreenInfo($"编译DLL成功，平台：{EditorUserBuildSettings.activeBuildTarget}");

		string desFolder = HOTFIX_DLL_FOLDER;
		if (Directory.Exists(desFolder)) Directory.Delete(desFolder, true);
		Directory.CreateDirectory(desFolder);

		List<string> failList = new List<string>();
		CopyHotfixDll(desFolder, ref failList);
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		if (failList.Count > 0) return;
		failList.Clear();
		Log.GreenInfo($"Hotfix DLL拷贝成功，{desFolder}");

		if (copyAotDll)
		{
			failList.Clear();
			CopyAOTDlls(ref failList);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}
		if (failList.Count > 0) return;

		Log.GreenInfo($"AOT DLL拷贝成功，{AOT_DLL_FOLDER}");
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
		string aotDesDir = AOT_DLL_FOLDER;
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
	}
}
