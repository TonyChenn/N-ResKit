using NCore;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class VersionInfo
{
	public Int32 bigVersion;
	public Int32 smallVersion;
	public string md5;
	public string appleExamVersion;
	public string cdn1;
	public string cdn2;
	public long time;

	public int VersionNumber => bigVersion * 100 + smallVersion;
}

public static class VersionInfoHelper
{
	public static void SerializeAndSave(this VersionInfo data, string savePath)
	{
		if (data == null) return;

		using MemoryStream stream = new();
		BinaryFormatter formatter = new();
		formatter.Serialize(stream, data);
		File.WriteAllBytes(savePath, stream.GetBuffer());
	}

	public static VersionInfo DeSerialize(string path)
	{
		var request = UnityWebRequest.Get(path);
		request.SendWebRequest();
		while (!request.isDone) { if (request.error != null) { return null; } }
		if (request.error != null) { return null; }

		return DeSerialize(request.downloadHandler.data);
	}
	public static VersionInfo DeSerialize(byte[] buffer)
	{
		using MemoryStream stream = new(buffer);
		stream.Position = 0;
		BinaryFormatter formatter = new();
		return formatter.Deserialize(stream) as VersionInfo;
	}
}
