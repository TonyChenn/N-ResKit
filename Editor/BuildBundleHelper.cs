using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NCore;
using UnityEditor;

namespace BuildBundle.Editor
{
    public static class BuildBundleHelper
    {
        public static string[] GetAssetsPath(string folder, string filter)
        {
            if (string.IsNullOrEmpty(folder))
                throw new ArgumentException("folder");
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("filter");

            if (filter.StartsWith("t:"))
            {
                string[] guids = AssetDatabase.FindAssets(filter, new string[] {folder});
                string[] paths = new string[guids.Length];
                for (int i = 0; i < guids.Length; i++)
                    paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                return paths;
            }
            else if (filter.StartsWith("f:"))
            {
                string[] files = Directory.GetFiles(folder.AssetPathToFullPath(), filter.Substring(2),
                    SearchOption.AllDirectories);
                string[] paths = new string[files.Length];

                for (int i = 0; i < files.Length; i++)
                    paths[i] = files[i].FullPathToAssetPath();
                
                return paths;
            }
            else if (filter.StartsWith("r:"))
            {
                string folderFullPath = folder.FullPathToAssetPath();
                string pattern = filter.Substring(2);
                string[] files = Directory.GetFiles(folderFullPath, "*.*", SearchOption.AllDirectories);
                List<string> list = new List<string>();
                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileName(files[i]);
                    if (Regex.IsMatch(name, pattern))
                    {
                        string p = files[i].FullPathToAssetPath();
                        list.Add(p);
                    }
                }

                return list.ToArray();
            }
            else
            {
                throw new InvalidOperationException("Unexpected filter: " + filter);
            }
        }
    }
}