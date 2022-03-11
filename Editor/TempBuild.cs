using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TempBuild
{
    [MenuItem("Tools/旧的打包")]
    public static void B()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
    }
}
