using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBundle
{
    public class UIPrefab : BuildingBundle
    {
        string m_srcFolder;
        string[] m_assetPaths;
        string m_name;

        public UIPrefab(string srcFolder, string folderPath, string[] assetPaths, string outputFolder)
            : base(outputFolder)
        {
            if (string.IsNullOrEmpty(folderPath) || !folderPath.StartsWith("Assets"))
                throw new ArgumentException("folderPath");
            if (assetPaths == null)
                throw new ArgumentNullException("assetPaths");
            if (assetPaths.Length < 1)
                throw new ArgumentException(string.Format("assetPaths.Length < 1, folderPath: {0}", folderPath));

            m_srcFolder = srcFolder;
            m_assetPaths = assetPaths;
            m_name = Path.GetFileNameWithoutExtension(folderPath);
        }

        protected override string ComputeHash()
        {
            List<byte> list = new List<byte>();
            foreach (var p in m_assetPaths)
            {
                byte[] buffer = ReadAssetBytes(p);
                if (buffer != null)
                    list.AddRange(buffer);
            }

            // 依赖项
            string[] dependencies = AssetDatabase.GetDependencies(m_assetPaths);
            foreach (var d in dependencies)
            {
                string name = Path.GetFileNameWithoutExtension(d);
                //if (name != CommonAtlasLoader.CommonAtlasName && name != CommonFontLoader.CommonFontName)       // 除去通用图集和通用字体，因为打包时去掉对于它们的引用
                //{
                    byte[] bufferOfD = ReadAssetBytes(d);
                    if (bufferOfD != null)
                        list.AddRange(bufferOfD);
                //}
            }

            return ComputeHash(list.ToArray());
        }

        public override string Name
        {
            get { return m_name; }
        }

        public override string[] AssetNames
        {
            get
            {
                if (!NeedBuild)
                    return null;

                if (m_assetPaths.Length < 0)
                {
                    Debug.LogError("No asset: " + Name);
                    return null;
                }

                string[] tempAssets = new string[m_assetPaths.Length];

                for (int i = 0; i < m_assetPaths.Length; i++)
                {
                    string assetPath = m_assetPaths[i];
                    tempAssets[i] = GetTempAssetPath(assetPath);
                }

                return tempAssets;
            }
        }

        string GetTempAssetPath(string assetPath)
        {
            return assetPath.Replace(m_srcFolder, UIPrefabs.TempFolder);
        }

        public void PrepareBuild()
        {
            if (m_assetPaths.Length < 0)
            {
                Debug.LogError("No asset: " + Name);
                return;
            }

            for (int i = 0; i < m_assetPaths.Length; i++)
            {
                string assetPath = m_assetPaths[i];
                GameObject obj = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);

                // 复制一份出来
                string newPath = GetTempAssetPath(assetPath);
                FolderHelper.CreateFileFolder(newPath);
                GameObject tempObj = PrefabUtility.SaveAsPrefabAsset(obj, newPath);

                // 去掉 common 图集
                //foreach (var sprite in sprites)
                //{
                //    if (sprite.atlas != null && sprite.atlas.name.ToLower() == CommonAtlasLoader.CommonAtlasName.ToLower())
                //    {
                //        sprite.atlas = null;
                //        CommonAtlasLoader loader = sprite.GetComponent<CommonAtlasLoader>();
                //        if (!loader)
                //            loader = sprite.gameObject.AddComponent<CommonAtlasLoader>();
                //    }
                //}

                //// 去掉通用字体
                //Text[] labels = tempObj.GetComponentsInChildren<Text>(true);
                //foreach (var label in labels)
                //{
                //    if ((label.bitmapFont != null && label.bitmapFont.name.ToLower() == CommonFontLoader.CommonFontName.ToLower())
                //        || (label.ambigiousFont != null && label.ambigiousFont.name.ToLower() == CommonFontLoader.CommonFontName.ToLower()))
                //    {
                //        label.bitmapFont = null;
                //        label.ambigiousFont = null;
                //        CommonFontLoader loader = label.GetComponent<CommonFontLoader>();
                //        if (!loader)
                //            loader = label.gameObject.AddComponent<CommonFontLoader>();
                //    }
                //}
            }
        }

        static bool ContainChinese(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            string pattern = "[\u4e00-\u9fbb]";
            return Regex.IsMatch(input, pattern);
        }

    }
}
