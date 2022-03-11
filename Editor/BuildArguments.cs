using UnityEngine;

namespace ResKit.Editor
{
    public class BuildArguments
    {
        /// <summary>
        /// 是否转表
        /// </summary>
        public bool ConvertTable = true;

        /// <summary>
        /// XLua
        /// </summary>
        public bool GenXLua = true;

        /// <summary>
        /// 是否上传到服务器
        /// </summary>
        public bool UploadToFTP = false;

        /// <summary>
        /// 打包工程
        /// </summary>
        public bool BuildProject = true;
        /// <summary>
        /// 全部重新打包（否则为增量打包）
        /// </summary>
        public bool BuildAll = false;

        /// <summary>
        /// ExcelFolder
        /// </summary>
        public string ExcelFolder = "../../Excel";
    }
}