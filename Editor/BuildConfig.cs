using System.Collections.Generic;
using System.Linq;
using UObject = UnityEngine.Object;

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
            // 通用图集
            yield return new SingleFiles<UObject>("Assets/BuildBundle/UI/Atlas/atlas_common", "t:prefab", "ui/");
            // UI 预设
            yield return new UIPrefabs("Assets/BuildBundle/UI/Prefabs", "t:Prefab", "ui/");
            // TextStyle
            yield return new SingleFiles<UObject>("Assets/BuildBundle/Asset/TextStyle", "f:*.asset", "asset/table/");
            // 配置表
            yield return new SingleFiles<UObject>("Assets/BuildBundle/Asset/Table", "f:*.asset", "asset/table/");
            // 图集
            // 贴图
            // lua
            yield return new LuaCodes("Assets/Scripts/XLua/LuaScripts", "*.lua", "lua/");
        }
    }
}