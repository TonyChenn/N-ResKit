using System.IO;
using UnityEditor;
using UnityEngine;

namespace ResKit.Editor
{
    public class ResEditorMenu
    {
        [MenuItem("Tools/打包AssetBundle/到StreammingAsset")]
        static void BuildToStreamming()
        {
            string outputPath = $"{Application.streamingAssetsPath}/AssetBundles/{BundlePathUtil.CurPlatformName}";
            BuildAB.Build(outputPath);
        }

        [MenuItem("Tools/打包AssetBundle/到Database")]
        static void BuildToDataBase()
        {
            string outputPath = $"{Path_BuildBundle.BundleDBRoot}/{BundlePathUtil.CurPlatformName}";
            BuildAB.Build(outputPath);
        }

        [MenuItem("Tools/打开文件夹/Bundle库")]
        static void OpenBundleFolder()
        {
            string folder = Path_BuildBundle.BundleDBRoot;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            Application.OpenURL($"file:///{folder}");
        }

        [MenuItem("Tools/打开文件夹/PersistentDataPath")]
        static void OpenPersistentFolder()
        {
            string folder = Application.persistentDataPath;
            Application.OpenURL($"file:///{folder}");
        }
    }
}