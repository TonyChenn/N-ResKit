using System.IO;
using UnityEditor;
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
		byte[] buffer = File.ReadAllBytes(path);
		using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
		{
			string small = reader.ReadInt32().ToString();
			string big = reader.ReadInt32().ToString();
			GUILayout.BeginHorizontal();
			GUILayout.Label("版本号: ", GUILayout.Width(90));
			GUILayout.TextField($"{big}.{small}");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("iOS提审版本号: ", GUILayout.Width(90));
			GUILayout.TextField(reader.ReadString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("版本MD5: ", GUILayout.Width(90));
			GUILayout.TextField(reader.ReadString());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("时间: ", GUILayout.Width(90));
			GUILayout.TextField(reader.ReadInt64().ToString());
			GUILayout.EndHorizontal();
		}
	}
}

