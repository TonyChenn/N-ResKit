using System.IO;

public class UIPrefab : BaseBundle
{
    private string m_SrcFolder;
    private string[] m_AssetPaths;
    private string m_Name;

    public UIPrefab(string srcFolder,string subFolder,string[] assetPaths, string outputFolder)
        : base(outputFolder)
    {
        m_SrcFolder = srcFolder;
        m_AssetPaths = assetPaths;
        m_Name = Path.GetFileNameWithoutExtension(subFolder);
    }

    public override string Name => m_Name;

    public override string[] AssetNames
    {
        get
        {
            if(m_AssetPaths.Length== 0) return null;

            //string[] result = new string[m_AssetPaths.Length];
            //for (int i = 0; i < m_AssetPaths.Length; i++)
            //{
            //    string path = m_AssetPaths[i];
            //    result[i] = path.Replace(m_SrcFolder, UIPrefabs.TempFolder);
            //}
            return m_AssetPaths;
        }
    }

    public void PrepareBuild()
    {
        if(m_AssetPaths.Length == 0 ) return;

        //for (int i = 0,iMax = m_AssetPaths.Length; i < iMax; i++)
        //{
        //    var path = m_AssetPaths[i];
        //    var obj = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
        //    // 临时复制出来一份
        //    var temp_path = path.Replace(m_SrcFolder, UIPrefabs.TempFolder);
        //    if (!Directory.Exists(temp_path)) { Directory.CreateDirectory(temp_path); }
        //    var temp_obj = PrefabUtility.SaveAsPrefabAsset(obj, temp_path);

        //    // 去掉通用图集
        //    // 去掉通用字体
        //    Text[] texts = temp_obj.GetComponentsInChildren<Text>();
        //    foreach (var item in texts)
        //    {

        //    }
        //}
    }
}
