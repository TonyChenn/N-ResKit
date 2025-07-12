using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MarkData : ScriptableObject
{
    public List<MarkItem> Items = new List<MarkItem>();

    private static MarkData _instance = null;
    public static MarkData Singlton
    {
        get
        {
            if (_instance == null)
            {
                string path = "Assets/Modules/N-ResKit/Editor/MarkBundleWnd/MarkData.asset";
                _instance = AssetDatabase.LoadAssetAtPath<MarkData>(path);
                if (_instance == null)
                {
                    MarkData table = ScriptableObject.CreateInstance<MarkData>();
                    AssetDatabase.CreateAsset(table, path);
                    AssetDatabase.Refresh();
                    _instance = table;
                }
            }
            return _instance;
        }
    }

    public MarkItem GetItemByName(string name)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Name == name)
                return Items[i];
        }
        MarkItem item = new MarkItem();
        item.Name = name;
        item.Type = MarkType.None;
        Items.Add(item);
        return item;
    }

    public void Modify(MarkItem item)
    {
        if (item == null) return;
        MarkItem temp = GetItemByName(item.Name);
        if (temp == null)
            Items.Add(item);
        else
        {
            temp.Name = item.Name;
            temp.Path = item.Path;
            temp.Type = item.Type;
        }
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }
}

[Serializable]
public class MarkItem
{
    public string Name;
    public string Path;
    public MarkType Type;

    public MarkItem() { }
    public MarkItem(string name, string path, MarkType type)
    {
        Name = name;
        Path = path;
        Type = type;
    }
}


public enum MarkType
{
    None,           // 未标记
    File,           // 单个文件打包
    Folder,         // 整个文件夹打包
    SubFolder,      // 每个子文件夹单独打包
    SubFiles,       // 子文件单独打包
    Raw,            // 原始文件
}
