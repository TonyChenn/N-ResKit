using System.Collections;
using System.Diagnostics;
using System.IO;
using Table.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ResKit.Editor
{
    public class BuildUI : EditorWindow
    {
        private Vector2 pos;
        private BuildArguments Args = new BuildArguments();
        private GUIStyle titleStyle;

        #region life method
        private void Awake()
        {
            titleStyle = new GUIStyle();
            titleStyle.normal.textColor=Color.red;
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
            GUILayout.TextField(Path_BuildBundle.BundleDBRoot);
            if (GUILayout.Button("修改", GUILayout.Width(50)))
                SettingWnd.ShowWindow("打包Bundle");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("表格目录：", GUILayout.Width(150));
            Args.ExcelFolder = GUILayout.TextField(Path_TableConfig.ExcelFolder);
            if (GUILayout.Button("修改", GUILayout.Width(50)))
                SettingWnd.ShowWindow("导表配置");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("打包平台：",GUILayout.Width(150));
            GUILayout.TextField(BundlePathUtil.CurPlatformName);
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            
            
            
            GUILayout.Label("打包参数：", titleStyle);
            Args.BuildProject = GUILayout.Toggle(Args.BuildProject, "打包工程");
            GUILayout.Space(10);
            
            Args.BuildAll = GUILayout.Toggle(Args.BuildAll, "全部重新打包");
            GUILayout.Space(10);
            
            Args.ConvertTable = GUILayout.Toggle(Args.ConvertTable, "导入Excel表数据", GUILayout.Width(150));
            GUILayout.Space(10);
            
            Args.GenXLua = GUILayout.Toggle(Args.GenXLua, "生成xLua");
            GUILayout.Space(10);
            
            Args.UploadToFTP = GUILayout.Toggle(Args.UploadToFTP, "上传资源");
            GUILayout.Space(10);
            
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("执行",GUILayout.Height(50)))
            {
                Build();
            }
        }
        #endregion
        
        #region editor menu
        [MenuItem("Tools/打包AssetBundle/一键打包")]
        private static void ShowWindow()
        {
            var window = GetWindow<BuildUI>();
            window.titleContent = new GUIContent("一键打包");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }
        #endregion
        
        public void Build()
        {
            IEnumerator etor = BuildExcute(Args, true);
            while (etor.MoveNext())
            {
                Debug.Log(etor.Current);
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

            if (Args.GenXLua)
            {
                // 奇奇怪怪，调用不到下面方法
                // CSObjectWrapEditor.Generator.GenAll();
                // 那就执行菜单吧
                EditorApplication.ExecuteMenuItem("XLua/Generate Code");
            }
            
            // TODO 转换配置表
            if (args.ConvertTable)
            {
                yield return ">>>>>>>>>>执行转换配置表操作..";
            }

            // 创建打包的文件夹
            string targetFolder = Path_BuildBundle.BundleDBRoot + "/" + BundlePathUtil.CurPlatformName;
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            
            // 开始打包操作
            var builder = BuildAB.Build(targetFolder, Args.BuildAll);
            while (builder.MoveNext())
                yield return builder.Current;

            AssetDatabase.Refresh();

            
            // 复制到StreammingAsset
            
            
            // TODO 上传资源到服务器
            if (args.UploadToFTP)
            {
                yield return ">>>>>>>>>>上传到服务器...";
            }

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
}