using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ǰ�ļ���
/// </summary>
public class CurFolderSearch : IBundleSearch
{
    public string[] SearchBundles(string folderPath)
    {
        return new string[] { folderPath };
    }
}
