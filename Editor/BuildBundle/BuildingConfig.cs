using System.Collections.Generic;

public static class BuildingConfig
{
    public static List<AssetCategory> GetAllConfig()
    {
		return new List<AssetCategory>()
		{
            // test non-build asset
			//new NonBuildAsset("Assets/BuildBundle/Code/StandaloneWindows64", "f:*.bytes", "dll"),

            // UIPrefab/字体/图集/散图
            new UIPrefabs("Assets/BuildBundle/UI/Prefabs","ui/prefabs"),
			new NormalAssets("Assets/BuildBundle/UI/Font","f:*.ttf","ui/font"),
			new NormalAssets("Assets/BuildBundle/UI/Image","t:Texture2D","ui/image"),


            // config/table
			//new NormalAssets<UObject>("Assets/BuildBundle/Asset/Config","f:*.asset","asset/config"),
            new NormalAssets("Assets/BuildBundle/Asset/Table","f:*.asset","asset/table"),

			// Code
            new DLLCodes("Assets/BuildBundle/Code", "code"),

            // Scenes
            new NormalAssets("Assets/BuildBundle/Scenes","t:Scene","scenes"),

            // Sounds
            new NormalAssets("Assets/BuildBundle/Sounds","t:AudioClip","sounds"),

            // video
            new NormalAssets("Assets/BuildBundle/Videos","t:VideoClip","video"),

        };
    }
}
