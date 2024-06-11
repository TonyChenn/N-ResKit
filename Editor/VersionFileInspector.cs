using System.IO;
using UnityEngine;

/// < summary >
/// 版本文件自定义Inspector面板
/// </ summary >
public class VersionFileInspector : ICustomDefaultAssetInspector
{
	public bool CanDraw(string path)
	{
		string fileName = Path.GetFileName(path);
		return fileName.StartsWith("version") && fileName.EndsWith(".data");
	}

	public void Draw(string path)
	{
		VersionInfo info = VersionInfoHelper.DeSerialize(PathUtil.GetAbsolutePath(path));
		if(info == null)
		{
			GUILayout.Label("版本文件已损坏！！");
			return;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("版本号: ", GUILayout.Width(90));
		GUILayout.TextField($"{info.bigVersion}");
		GUILayout.TextField($"{info.smallVersion}");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("iOS提审版本号: ", GUILayout.Width(90));
		GUILayout.TextField(info.appleExamVersion);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("版本MD5: ", GUILayout.Width(90));
		GUILayout.TextField(info.md5);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("CDN1: ", GUILayout.Width(90));
		GUILayout.TextField(info.cdn1);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("CDN2: ", GUILayout.Width(90));
		GUILayout.TextField(info.cdn2);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("时间: ", GUILayout.Width(90));
		GUILayout.TextField(info.time.ToString());
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
	}
}

