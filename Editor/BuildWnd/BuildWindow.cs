using System.Collections;
using System.Diagnostics;
using System.IO;
using Table.Editor;
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
        pos = GUILayout.BeginScrollView(pos);
        // 配置目录
        GUILayout.Label("配置信息：", titleStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("AB包库：", GUILayout.Width(150));
        Path_BuildBundle.SelectedBuildPlaceIndex = GUILayout.SelectionGrid(Path_BuildBundle.SelectedBuildPlaceIndex, buildBundlePlaceArray, buildBundlePlaceArray.Length);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(150));
        Path_BuildBundle.BuildBundleFolderPath = GUILayout.TextField(Path_BuildBundle.BuildBundleFolderPath);
        if (Path_BuildBundle.SelectedBuildPlaceIndex == 0)
            Path_BuildBundle.BuildBundleFolderPath = Application.streamingAssetsPath;
        else if (Path_BuildBundle.SelectedBuildPlaceIndex == 1)
            Path_BuildBundle.BuildBundleFolderPath = Path.GetFullPath($"{Application.dataPath}/../../AssetBundleDB/{Args.CurBuildTarget.ToString()}");
        else if (Path_BuildBundle.SelectedBuildPlaceIndex == 2)
        {
            if (GUILayout.Button("修改", GUILayout.Width(50)))
            {
                Path_BuildBundle.BuildBundleFolderPath = EditorUtility.SaveFolderPanel("选择导出AssetBundle目录", Path_BuildBundle.BuildBundleFolderPath, Path_BuildBundle.BuildBundleFolderPath);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("表格目录：", GUILayout.Width(150));
        Args.ExcelFolder = GUILayout.TextField(Path_TableConfig.ExcelFolder);
        if (GUILayout.Button("设置", GUILayout.Width(50)))
        {
            PackageWnd.ShowWnd(SettingPage.TAG, "导表配置");
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("打包平台：", GUILayout.Width(150));
        Args.CurBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(Args.CurBuildTarget);
        GUILayout.EndHorizontal();
        GUILayout.Space(30);



        GUILayout.Label("打包参数：", titleStyle);
        Args.BuildProject = GUILayout.Toggle(Args.BuildProject, "打包工程");
        GUILayout.Space(5);

        Args.BuildAll = GUILayout.Toggle(Args.BuildAll, "全部重新打包");
        GUILayout.Space(5);

        Args.ConvertTable = GUILayout.Toggle(Args.ConvertTable, "导入Excel表数据", GUILayout.Width(150));
        GUILayout.Space(5);

        Args.GenHybirdCLR = GUILayout.Toggle(Args.GenHybirdCLR, "生成HybirdCLR");
        if(Args.BuildProject || Args.BuildAll)
        {
            Args.GenHybirdCLR = true;
        }
        GUILayout.Space(5);

        Args.CopyToSreeammingAsset = GUILayout.Toggle(Args.CopyToSreeammingAsset, "复制Bundle到StreammingAsset");
        GUILayout.Space(5);

        Args.UploadToFTP = GUILayout.Toggle(Args.UploadToFTP, "上传资源");
        GUILayout.Space(5);

        Args.BuildUI = GUILayout.Toggle(Args.BuildUI, "打包UI");
        GUILayout.Space(5);

        GUILayout.EndScrollView();

        if (GUILayout.Button("执行", GUILayout.Height(50)))
        {
            Build();
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
        System.Collections.IEnumerator etor = BuildExcute(Args, true);
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
        // TODO 转换配置表
        if (args.ConvertTable)
        {
            yield return ">>>>>>>>>>执行转换配置表操作..";
        }

        if (Args.GenHybirdCLR)
        {
            BuildTarget target = Args.CurBuildTarget;
            UnityEngine.Debug.Log("打包平台：" + target.ToString());
            BuildDll.CompileDll(target);
            BuildDll.BuildDllBundle(target);
        }

        // 创建打包的文件夹
        if (!Directory.Exists(Path_BuildBundle.BuildBundleFolderPath))
            Directory.CreateDirectory(Path_BuildBundle.BuildBundleFolderPath);

        // 开始打包操作
        if (Args.BuildProject)
        {
            var builder = BuildAB.Build(Path_BuildBundle.BuildBundleFolderPath, Args.BuildAll);
            while (builder.MoveNext())
            {
                yield return builder.Current;
            }
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);


        // 复制到StreammingAsset
        if (Args.CopyToSreeammingAsset)
        {

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

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