using NCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResKit
{
    public static class PathUtil
    {
        private static string temp_str = string.Empty;

        public static string GetAssetBundlePath(string bundleName)
        {
            if (!GameConfig.UseLocalAsset)
            {
                string path = GetAssetBundlePersistPath(bundleName);
                if (System.IO.File.Exists(path))
                    return path;
            }
            return GetAssetBundleStreammingPath(bundleName);
        }

        #region 

        /// <summary>
        /// 从Persistent获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetAssetBundlePersistPath(string path)
        {
            var builder = StringBuilderPool.Alloc();
            builder.Append(Application.persistentDataPath);
            builder.Append("/");
            builder.Append(path);
            temp_str = builder.ToString();
            builder.Recycle();
            return temp_str;
        }

        private static string GetAssetBundleStreammingPath(string path)
        {
            var builder = StringBuilderPool.Alloc();
            builder.Append(Application.streamingAssetsPath);
            builder.Append("/");
            builder.Append(path);
            temp_str = builder.ToString();
            builder.Recycle();
            return temp_str;
        }
        #endregion
    }
}

