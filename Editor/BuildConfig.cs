using System.Collections.Generic;
using System.Linq;

namespace BuildBundle.Editor
{
    public static class BuildConfig
    {
        public static List<AssetCategory> GetNeedBuildAssetList()
        {
            var config = GetBuildABConfig();
            return config?.ToList();
        }

        private static IEnumerable<AssetCategory> GetBuildABConfig()
        {
            // UI 预设
            yield return new UIPrefabs("Assets/UI/Prefabs", "t:Prefab", "ui/");
            // 字体
            // 图集
            // 贴图
            // lua
            yield return new LuaCodes("Assets/Scripts/XLua/LuaScripts", "*.lua", "lua");
        }
    }
}