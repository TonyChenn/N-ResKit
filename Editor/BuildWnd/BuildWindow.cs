using NDebug;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildWindow : EditorWindow
{
	private Vector2 pos;
	private BuildArguments Args = new BuildArguments();
	private GUIStyle titleStyle;
	private string[] buildBundlePlaceArray = new string[] { "StreammingAsset", "AB包库", "自定义" };


	#region life method
	private void Awake()
	{
		titleStyle = new GUIStyle();
		titleStyle.normal.textColor = Color.red;
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.fontSize = 20;
	}
	private void OnGUI()
	{
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.BeginVertical();
		DrawMenu();
		GUILayout.EndVertical();
		GUILayout.Space(20);
		GUILayout.EndHorizontal();
	}
	void DrawMenu()
	{
		pos = GUILayout.BeginScrollView(pos);
		// 配置目录
		GUILayout.Label("配置信息：", titleStyle);

		GUILayout.BeginHorizontal();
		GUILayout.Label("表格目录：", GUILayout.Width(150));
		GUILayout.TextField(Path_TableConfig.ExcelFolder);
		if (GUILayout.Button("设置", GUILayout.Width(50)))
		{
			PackageWnd.ShowWnd(SettingPage.TAG, "导表配置");
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label($"打包平台：", GUILayout.Width(150));
		GUILayout.TextField(EditorUserBuildSettings.activeBuildTarget.ToString());
		GUILayout.EndHorizontal();

		GUILayout.Space(20);

		GUILayout.BeginHorizontal();
		GUILayout.Label("输出目录：", GUILayout.Width(150));
		Path_BuildBundle.SelectedBuildPlaceIndex = GUILayout.SelectionGrid(Path_BuildBundle.SelectedBuildPlaceIndex, buildBundlePlaceArray, buildBundlePlaceArray.Length);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("", GUILayout.Width(150));
		Path_BuildBundle.BuildBundleFolderPath = GUILayout.TextField(Path_BuildBundle.BuildBundleFolderPath);
		if (Path_BuildBundle.SelectedBuildPlaceIndex == 0)
			Path_BuildBundle.BuildBundleFolderPath = Application.streamingAssetsPath;
		else if (Path_BuildBundle.SelectedBuildPlaceIndex == 1)
		{
			string path = $"{Application.dataPath}/../../AssetBundleDB/{EditorUserBuildSettings.activeBuildTarget}";
			Path_BuildBundle.BuildBundleFolderPath = Path.GetFullPath(path);

		}
		else if (Path_BuildBundle.SelectedBuildPlaceIndex == 2)
		{
			if (GUILayout.Button("修改", GUILayout.Width(50)))
			{
				Path_BuildBundle.BuildBundleFolderPath = EditorUtility.SaveFolderPanel("选择导出AssetBundle目录", Path_BuildBundle.BuildBundleFolderPath, Path_BuildBundle.BuildBundleFolderPath);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10);


		GUILayout.Label("打包参数：", titleStyle);

		EditorGUI.BeginDisabledGroup(true);
		Args.MD5BundleName = GUILayout.Toggle(Path_BuildBundle.BuildBundleFolderPath != Application.streamingAssetsPath, "修改Bundle名为MD5");
		EditorGUI.EndDisabledGroup();
		GUILayout.Space(5);

		Args.UploadToFTP = GUILayout.Toggle(Args.UploadToFTP, "上传资源");
		GUILayout.Space(5);

		GUILayout.EndScrollView();

		if (GUILayout.Button("执行", GUILayout.Height(50)))
		{
			Build();
			GUIUtility.ExitGUI();
		}
	}
	#endregion

	#region editor menu
	[MenuItem("Tools/AssetBundle/一键打包工具")]
	public static void ShowWindow()
	{
		var window = Create();
		window.titleContent = new GUIContent("一键打包");
		window.minSize = new Vector2(500, 300);
		window.Show();
	}
	public static BuildWindow Create()
	{
		return GetWindow<BuildWindow>();
	}
	#endregion

	public void Build()
	{
		IEnumerator etor = BuildExcute(Args, true);
		while (etor.MoveNext())
		{
			UnityEngine.Debug.Log(etor.Current);
		}
	}

	public IEnumerator BuildExcute(BuildArguments args, bool showDialog)
	{
		if (showDialog)
		{
			if (!EditorUtility.DisplayDialog("一键打包工具", "确定要执行吗？", "确定", "取消"))
				yield break;
		}

		//开始计时
		Stopwatch watch = new Stopwatch();
		watch.Start();


		#region 打包流程
		// 创建打包的文件夹
		if (!Directory.Exists(Path_BuildBundle.BuildBundleFolderPath))
			Directory.CreateDirectory(Path_BuildBundle.BuildBundleFolderPath);

		// 打包
		var builder = Task_BuildAB.Build(BuildingConfig.GetAllConfig(), Path_BuildBundle.BuildBundleFolderPath);
		while (builder.MoveNext())
		{
			yield return builder.Current;
		}
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		// build dll
		Log.Info("HybridCLR打包平台：" + EditorUserBuildSettings.activeBuildTarget);
		Task_CompileDLL.CompileAndCopyDll();
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		// 生成版控文件/资源MD5文件
		Task_CreateVersionInfo.CreateHashFile();
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		// TODO 上传资源到服务器
		if (args.UploadToFTP)
		{
			yield return ">>>>>>>>>>上传到服务器...";
		}
		#endregion

		watch.Stop();

		int totalSeconds = (int)(watch.ElapsedMilliseconds / 1000f);
		int minutes = Mathf.FloorToInt(totalSeconds / 60f);
		int seconds = totalSeconds % 60;
		if (showDialog)
		{
			EditorUtility.DisplayDialog("打包完成", $"执行结束！耗时： {minutes} 分 {seconds} 秒。", "好的");
		}
	}
}
