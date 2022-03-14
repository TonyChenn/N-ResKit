using NCore.Editor;
using System.IO;
using UnityEngine;

namespace ResKit.Editor
{
    public class Path_BuildBundle : IPathConfig, IEditorPrefs
    {
        /// <summary>
        /// AB包库根目录
        /// </summary>
        
        [SettingProperty(FieldType.Folder,"Bundle库：")]
        public static string BundleDBRoot
        {
            get
            {
                string path = EditorPrefsHelper.GetString("Path_BuildBundle_BundleDBRoot", "");
                if (string.IsNullOrEmpty(path))
                {
                    path = $"{Application.dataPath}/../../AssetBundles";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }

                return path;
            }
            set { EditorPrefsHelper.SetString("Path_BuildBundle_BundleDBRoot", value.TrimEnd('/')); }
        }

        [SettingMethod("打包配置信息: ", "打开配置文件")]
        public static void OpenCfgFile()
        {
            string path = Application.dataPath + "/Modules/N-ResKit/Editor/BuildConfig.cs";
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
        }

        /// <summary>
        /// 打包临时目录
        /// </summary>
        //[Setting(FieldType.TextField, "临时目录(相对路径)：")]
        //public static string TempBuildFolder => $"{BundleDBRoot}/Temp";

        //[Setting(FieldType.TextField, "打包结果Hash：")]
        //public static string BuildResultHashFile => $"{BundleDBRoot}/{BundlePathUtil.CurPlatformName}/main.dat";

        //[Setting(FieldType.TextField, "资源更新配置信息：")]
        //public static string UpdateConfigFile => $"{BundleDBRoot}/{BundlePathUtil.CurPlatformName}/update.dat";


        #region IPathConfig,IEditorPrefs

        public string GetModuleName()
        {
            return "打包Bundle";
        }

        public void ReleaseEditorPrefs()
        {
            EditorPrefsHelper.DeleteKey("Path_BuildBundle_BundleDBRoot");
        }

        #endregion
    }
}