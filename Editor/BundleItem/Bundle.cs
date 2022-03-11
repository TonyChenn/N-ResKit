using System;

namespace BuildBundle.Editor
{
    public abstract class Bundle
    {
        #region Enum

        public enum Flag
        {
            NoChange,
            NewAdded,
            Modified,
        }

        #endregion

        private string outputFolder;
        private string currentHash;
        private Flag flag = Flag.NoChange;

        public Bundle(string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("outputFolder is null or empty");

            this.outputFolder = outputFolder.TrimStart('/').TrimEnd('/');
        }

        public string OutputRelativePath
        {
            get
            {
                string path = outputFolder.Contains("{0}")
                    ? string.Format(outputFolder, Name)
                    : string.Format($"{outputFolder}/{Name}");
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
                if (string.IsNullOrEmpty(currentHash))
                    currentHash = ComputeHash();
                return currentHash;
            }
        }

        public bool NeedBuild
        {
            get { return CurrentHash != LastHash; }
        }

        public Flag BuildFlag
        {
            get { return flag; }
            set { flag = value; }
        }


        public void ComputeHashIfNeeded()
        {
            if (string.IsNullOrEmpty(currentHash))
                currentHash = ComputeHash();
        }

        #region 配置
        /// <summary>
        /// 生成一条配置
        /// </summary>
        public string GenConfigLine()
        {
            return $"{OutputRelativePath}{Ext},{CurrentHash}";
        }

        /// <summary>
        /// 解析一条配置
        /// </summary>
        public static void ParseConfigLine(string line, out string relativePath, out string lastHash)
        {
            string[] words = line.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            relativePath = words[0];
            lastHash = words[1];
        }

        /// <summary>
        /// 读取上次打包hash值
        /// </summary>
        /// <param name="configLines"></param>
        public void ReadConfig(string[] configLines)
        {
            string perfix = AssetBundleName + ",";
            for (int i = 0,iMax=configLines.Length; i < iMax; i++)
            {
                if (configLines[i].StartsWith(perfix))
                {
                    string _,lastHash;
                    ParseConfigLine(configLines[i],out _, out lastHash);
                    LastHash = lastHash;
                    break;
                }
            }
        }
        #endregion


        #region abstract

        public abstract string ComputeHash();
        
        /// <summary>
        /// 资源名称
        /// </summary>
        public abstract string Name { get; }
        public abstract string[] AssetNames { get; }

        #endregion

        #region virtual
        /// <summary>
        /// 拓展名
        /// </summary>
        public virtual string Ext { get { return ".u"; }}

        public virtual void Dispose() { }

        #endregion
    }
}