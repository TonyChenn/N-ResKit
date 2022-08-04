using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChangePlatformAssets
{
	public static string OutputPath = Application.dataPath + "/StreamingAssets";
	public static string SrcFolderPath(string platformName)
	{
		string folder = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
		string path = string.Format("{0}/AssetsLib/{1}", folder, platformName);
		return path.Replace("\\", "/");
	}
	[MenuItem("BuildTool/ChangeStreamingAsset/Android")]
	public static void ChangeAndroid()
	{
		BeginChange("android");
	}
	[MenuItem("BuildTool/ChangeStreamingAsset/Iphone")]
	public static void ChangeIphone()
	{
		BeginChange("iphone");
	}
	[MenuItem("BuildTool/ChangeStreamingAsset/Editor")]
	public static void ChangeEditor()
	{
		BeginChange("editor");
	}

	static void BeginChange(string platformName)
	{
		string srcPath = SrcFolderPath(platformName);
		if (!Directory.Exists(srcPath))
		{
			EditorUtility.DisplayDialog("资源拷贝Error", srcPath + " 平台路径没有找到！", "确定");
			return;
		}
		DirectoryInfo direction = new DirectoryInfo(srcPath);
		FileInfo[] files = direction.GetFiles("*", SearchOption.TopDirectoryOnly);
		if(files.Length == 0)
		{
			EditorUtility.DisplayDialog("资源拷贝Error", platformName + " 平台路径没有资源", "确定");
			return;
		}
		if (Directory.Exists(OutputPath))
		{
			Directory.Delete(OutputPath, true);
		}
		Directory.CreateDirectory(OutputPath);

		CopyFolder(srcPath, OutputPath);

		AssetDatabase.Refresh();
	}
	static void CopyFolder(string srcPath, string tarPath)
	{
		if (!Directory.Exists(srcPath))
		{
			Directory.CreateDirectory(srcPath);
		}
		if (!Directory.Exists(tarPath))
		{
			Directory.CreateDirectory(tarPath);
		}
		CopyFile(srcPath, tarPath);
		string[] directionName = Directory.GetDirectories(srcPath);
		foreach (string dirPath in directionName)
		{
			string directionPathTemp = tarPath + "\\" + dirPath.Substring(srcPath.Length + 1);
			CopyFolder(dirPath, directionPathTemp);
		}
	}
	static void CopyFile(string srcPath, string tarPath)
	{
		string[] filesList = Directory.GetFiles(srcPath);
		foreach (string f in filesList)
		{
			string fTarPath = tarPath + "\\" + f.Substring(srcPath.Length + 1);
			if (File.Exists(fTarPath))
			{
				File.Copy(f, fTarPath, true);
			}
			else
			{
				File.Copy(f, fTarPath);
			}
		}
	}
}