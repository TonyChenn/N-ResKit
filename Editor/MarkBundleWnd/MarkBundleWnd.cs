using UnityEditor;
using UnityEngine;
using System.IO;

public class MarkBundleWnd : EditorWindow
{
    private Vector2 pos;
    private GUIStyle labelStyle = null;
    private GUIStyle popupStyle = null;

    private TreeNode rootTreeNode = null;
    private DirectoryInfo rootDir = null;
    private MarkBundleTreeView treeView = null;

    private GUIStyle LabelStyle { get { if (labelStyle == null) labelStyle = new GUIStyle(EditorStyles.label); return labelStyle; } }
    private GUIStyle PopupStyle { get { if (popupStyle == null) popupStyle = new GUIStyle(EditorStyles.popup); return popupStyle; } }
    private TreeNode RootTreeNode { get { if (rootTreeNode == null) rootTreeNode = new TreeNode("Root", true); return rootTreeNode; } }
    private DirectoryInfo RootDir { get { if (rootDir == null) rootDir = new DirectoryInfo(Path_BuildBundle.BundleRootFolder); return rootDir; } }
    private MarkBundleTreeView TreeView { get { if (treeView == null) treeView = new MarkBundleTreeView(); return treeView; } }


    private void OnEnable()
    {
        RootTreeNode.children.Clear();
        refreshData(RootTreeNode, RootDir);
    }

    private void OnGUI()
    {
        pos = GUILayout.BeginScrollView(pos);
        TreeView.DrawUI(RootTreeNode, 1);
        GUILayout.EndScrollView();

        if (GUILayout.Button("保存"))
        {
            MarkData.Singlton.Save();
        }
    }

    #region RefreshTreeView
    private void refreshData(TreeNode node, DirectoryInfo folder)
    {
        DirectoryInfo[] dirs = folder.GetDirectories();
        FileInfo[] files = folder.GetFiles();
        for (int i = 0, iMax = dirs.Length; i < iMax; i++)
        {
            TreeNode item = new TreeNode(dirs[i].Name, true);
            item.isGroup = true;

            node.AddChild(item);
            refreshData(item, dirs[i]);
        }
        for (int i = 0, iMax = files.Length; i < iMax; i++)
        {
            if (files[i].Name.EndsWith(".meta")) continue;
            node.AddChild(new TreeNode(files[i].Name, false));
        }
    }
    #endregion

    #region MenuItem
    [MenuItem("Tools/AssetBundle/标记Bundle")]
    public static void ShowMarkBundleWind()
    {
        var window = GetWindow<MarkBundleWnd>();
        window.titleContent = new GUIContent("AssetBundle标记");
        window.minSize = new Vector2(500, 300);
        window.Show();
    }
    #endregion
}
