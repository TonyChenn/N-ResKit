using System;

namespace ResKit
{
    public static class ResFactory
    {
        /// <summary>
        /// 加载AB包和包内资源
        /// </summary>
        /// <param name="assetName">当assetName为空时，bundleName就是assetName</param>
        /// <param name="bundleName">当assetName为空时，bundleName就是assetName</param>
        /// <returns></returns>
        public static ResBase Create(string assetName, string bundleName = null)
        {
            ResBase res = null;
            if (!string.IsNullOrEmpty(bundleName))
            {
                res = new AssetRes(bundleName, assetName);
            }
            else if (assetName.StartsWith("resources://"))
            {
                res = new ResourcesRes(assetName);
            }
            else
            {
                res = new BundleRes(assetName);
            }
            return res;
        }
    }
}