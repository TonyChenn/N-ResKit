using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetBundle
{
    /// <summary>
    /// 单个的ab包
    /// </summary>
    public abstract class BuildingBundle
    {
        string m_outputFolder;
        string m_currentHash;
        Flag m_flag = Flag.NoChange;

        protected abstract string ComputeHash();
        public abstract string Name { get; }
        public abstract string[] AssetNames { get; }


        public enum Flag
        {
            NoChange, NewAdded, Modified,
        }


        #region static

        static MD5 s_md5Obj;
        static MD5 MD5Obj
        {
            get { return s_md5Obj ?? (s_md5Obj = MD5.Create()); }
        }

        public static void ParseConfigLine(string configLine, out string relativePath, out string hash)
        {
            string[] words = configLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            relativePath = words[0];
            hash = words[1];
        }

        /// <summary>
        /// asset path 转 full path
        /// </summary>
        public static string GetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return "";

            string p = Application.dataPath + assetPath.Substring(6);
            return p.Replace("\\", "/");
        }

        /// <summary>
        /// full path 转 asset path
        /// </summary>
        public static string GetAssetPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return "";

            fullPath = fullPath.Replace("\\", "/");
            return fullPath.StartsWith("Assets/") ?
                fullPath :
                "Assets" + fullPath.Substring(Application.dataPath.Length);
        }

        public static byte[] ReadAssetBytes(string assetPath)
        {
            string fullPath = GetFullPath(assetPath);
            if (!File.Exists(fullPath))
                return null;

            List<byte> list = new List<byte>();

            var a = File.ReadAllBytes(fullPath);
            list.AddRange(a);

            string metaPath = fullPath + ".meta";
            var b = File.ReadAllBytes(metaPath);
            list.AddRange(b);

            return list.ToArray();
        }

        /// <summary>
        /// 计算哈希值字符串
        /// </summary>
        public static string ComputeHash(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 1)
                return "";

            byte[] hash = MD5Obj.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();

            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// 计算单个资源的哈希码
        /// </summary>
        public static string ComputeHash(string assetPath)
        {
            byte[] buffer = ReadAssetBytes(assetPath);
            return buffer != null ? ComputeHash(buffer) : null;
        }

        /// <summary>
        /// 计算单个文件联合依赖项的哈希码
        /// </summary>
        public static string ComputeHashWithDependencies(string assetPath)
        {
            List<byte> list = ReadAssetWithDependenciesBytes(assetPath);
            return ComputeHash(list.ToArray());
        }

        public static List<byte> ReadAssetWithDependenciesBytes(string assetPath)
        {
            byte[] buffer = ReadAssetBytes(assetPath);
            if (buffer == null)
                return new List<byte>();

            List<byte> list = new List<byte>(buffer);

            // 依赖项
            string[] dependencies = AssetDatabase.GetDependencies(new string[] { assetPath });
            foreach (var d in dependencies)
            {
                byte[] bufferOfD = ReadAssetBytes(d);
                if (bufferOfD != null)
                    list.AddRange(bufferOfD);
            }

            return list;
        }

        /// <summary>
        /// 计算若干个文件合并成的哈希码
        /// </summary>
        public static string ComputeHashWithDependencies(string[] assetPaths)
        {
            List<byte> list = new List<byte>();
            foreach (var p in assetPaths)
            {
                byte[] buffer = ReadAssetBytes(p);
                if (buffer != null)
                    list.AddRange(buffer);
            }

            // 依赖项
            string[] dependencies = AssetDatabase.GetDependencies(assetPaths);
            foreach (var d in dependencies)
            {
                byte[] bufferOfD = ReadAssetBytes(d);
                if (bufferOfD != null)
                    list.AddRange(bufferOfD);
            }

            return ComputeHash(list.ToArray());
        }

        #endregion

        public BuildingBundle(string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("outputFolder");

            m_outputFolder = outputFolder.TrimStart('/').TrimEnd('/');
        }

        public void ComputeHashIfNeeded()
        {
            if (string.IsNullOrEmpty(m_currentHash))
                m_currentHash = ComputeHash();
        }

        /// <summary>
        /// 生成一条配置
        /// </summary>
        public string GenerateConfigLine()
        {
            return string.Format("{0}{1},{2},", OutputRelativePath, Ext, CurrentHash);
        }

        /// <summary>
        /// 读取配置，从中找出属于自己的那条
        /// </summary>
        public void ReadConfig(string[] configLines)
        {
            foreach (var l in configLines)
            {
                if (RightConfig(l))
                {
                    string r, lastHash;
                    ParseConfigLine(l, out r, out lastHash);
                    LastHash = lastHash;
                    break;
                }
            }
        }

        public bool RightConfig(string configLine)
        {
            return configLine.StartsWith(OutputRelativePath + Ext + ",");
        }

        public virtual void Dispose() { }

        public string OutputRelativePath
        {
            get
            {
                string path = m_outputFolder.Contains("{0}") ? string.Format(m_outputFolder, Name) : string.Format("{0}/{1}", m_outputFolder, Name);
                return path.ToLower();
            }
        }

        public string AssetBundleName
        {
            get { return OutputRelativePath + Ext; }
        }

        public string LastHash { get; set; }

        public string CurrentHash
        {
            get
            {
                if (string.IsNullOrEmpty(m_currentHash))
                    m_currentHash = ComputeHash();
                return m_currentHash;
            }
        }

        public string FullName
        {
            get { return Name + Ext; }
        }

        public bool NeedBuild
        {
            get { return CurrentHash != LastHash; }
        }

        public Flag BuildingFlag
        {
            set { m_flag = value; }
            get { return m_flag; }
        }

        public virtual string Ext
        {
            get { return ".u"; }
        }
    }
}
