using UnityEditor;
using UnityEngine;

public class MarkBundleTreeView : TreeView
{
    private GUIStyle labelStyle = null;
    private GUIStyle popupStyle = null;

    private GUIStyle LabelStyle
    {
        get { if(labelStyle == null) labelStyle = new GUIStyle(EditorStyles.label);  return labelStyle; }
    }
    private GUIStyle PopupStyle
    {
        get { if(popupStyle == null) popupStyle = new GUIStyle(EditorStyles.popup); return popupStyle; }
    }


    protected override void drawGroup(TreeNode node, int layer)
    {

        // 数据
        MarkItem parent = (MarkItem)node.parent.data;


        MarkItem item = MarkData.Singlton.GetItemByName(node.name);
        if (item == null)
        {
            item = new MarkItem();
            item.Name = node.name;
            item.Type = MarkType.None;
        }
        if (parent?.Type == MarkType.SubFolder)
            item.Type = MarkType.Folder;

        node.data = item;

        // ui
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();
        drawSpace(layer);
        GUILayout.Label(EditorGUIUtility.IconContent(getFolderIconName(node)), GUILayout.Width(20), GUILayout.Height(20));
        node.isOpen = EditorGUILayout.Foldout(node.isOpen, node.name, true);
        if (parent?.Type != MarkType.SubFolder) 
        {
            ((MarkItem)node.data).Type = (MarkType)EditorGUILayout.EnumPopup(((MarkItem)node.data).Type, GUILayout.Width(100));
            MarkData.Singlton.Modify((MarkItem)node.data);
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (node.isOpen)
        {
            for (int i = 0, iMax = node.children.Count; i < iMax; i++)
            {
                drawNode(node.children[i], layer + 1);
            }
        }
    }
    protected override void drawItem(TreeNode node, int layer)
    {
        MarkItem parent = (MarkItem)node.parent.data;

        GUILayout.BeginHorizontal("box");
        drawSpace(layer);
        GUILayout.Label(EditorGUIUtility.IconContent(getFileIconName(node.name)), GUILayout.Width(20), GUILayout.Height(20));
        GUILayout.Label(node.name);
        GUILayout.FlexibleSpace();
        if(parent!=null && parent.Type != MarkType.SubFiles && parent.Type != MarkType.Folder)
        {
            if (node.data == null) node.data = new MarkItem(node.name,"",MarkType.None);
            ((MarkItem)node.data).Type = (MarkType)EditorGUILayout.EnumPopup(((MarkItem)node.data).Type, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();
    }

    private string getFolderIconName(TreeNode node)
    {
        string folderName = node.name;
        if (folderName.Equals("StandaloneWindows") || folderName.Equals("StandaloneWindows64")
            || folderName.Equals("StandaloneOSX") || folderName.Equals("StandaloneLinux64"))
            return "BuildSettings.Standalone";
        else if (folderName.Equals("Android")) return "BuildSettings.Android";
        else if (folderName.Equals("iOS")) return "BuildSettings.iPhone";
        else if (folderName.StartsWith("atlas_")) return "SpriteAtlas Icon";
        else if (node.children.Count == 0) return "FolderEmpty Icon";
        else if (node.isOpen) return "FolderOpened Icon";
        else return "Folder Icon";
    }
    private string getFileIconName(string fileName)
    {
        fileName = fileName.ToLower();
        if (fileName.EndsWith(".png"))                                      // 场景
            return "RawImage Icon";
        else if (fileName.EndsWith(".unity"))                               // 场景
            return "BuildSettings.Editor.Small";
        else if (fileName.EndsWith("avi") || fileName.EndsWith("mp4"))      // 视频
            return "MovieTexture Icon";
        else if (fileName.EndsWith(".ttf"))                                 // 字体
            return "Font Icon";
        else if (fileName.EndsWith(".prefab"))                              // 预设
            return "Prefab Icon";
        else if (fileName.EndsWith(".asset") || fileName.EndsWith(".tss"))  // asset
            return "ScriptableObject Icon";
        else if (fileName.EndsWith(".txt"))                                 // 文本
            return "TextAsset Icon";
        else if (fileName.EndsWith(".json")|| fileName.EndsWith(".uss"))   // json
            return "StyleSheet Icon";
        else if (fileName.EndsWith(".dll.bytes"))                           // dll
            return "AssemblyDefinitionAsset Icon";
        else if (fileName.EndsWith(".shader"))                              // shader
            return "Shader Icon";
        else if (fileName.EndsWith(".xml") || fileName.EndsWith(".uxml"))   // xml
            return "UxmlScript Icon";
        else if (fileName.EndsWith(".mat"))                                // 材质球
            return "Material Icon";
        else
            return "DefaultAsset Icon";
    }
}
