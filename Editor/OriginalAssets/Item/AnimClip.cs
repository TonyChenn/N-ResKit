using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetBundle
{
    public class AnimClip : SingleFile<AnimationClip>
    {
        public const string TempFolder = "Assets/Resources/Temp";

        public AnimClip(string assetPath, string outputFolder, string srcFolder)
            : base(assetPath, outputFolder, srcFolder)
        {

        }

        public override string[] AssetNames
        {
            get { return new string[] { NewPath }; }
        }

        public void GenerateClip()
        {
            AnimationClip srcClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(base.AssetPath);
            AnimationClip tempClip = new AnimationClip();
            EditorUtility.CopySerialized(srcClip, tempClip);
            string newPath = NewPath;
            FolderHelper.CreateFileFolder(newPath);
            AssetDatabase.CreateAsset(tempClip, newPath);
        }

        string NewPath
        {
            get
            {
                string ext = Path.GetExtension(base.AssetPath);
                string pathWithoutExt = AssetPath.Substring(0, AssetPath.Length - ext.Length);
                return string.Format("{0}/{1}.anim", TempFolder, pathWithoutExt);
            }
        }

    }
}
