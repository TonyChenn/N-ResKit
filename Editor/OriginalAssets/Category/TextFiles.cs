using UObject = UnityEngine.Object;

namespace AssetBundle
{
    public class TextFiles : NormalAssets<UObject>
    {
        public TextFiles(string srcFolder, string filter, string outputFolder)
            : base(srcFolder, filter, outputFolder)
        {

        }
    }
}
