using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBundle
{
    string m_outputFolder;
    string m_hash;
    Flag m_flag = Flag.NoChange;

    public BaseBundle(string outputFolder)
    {
        m_outputFolder = outputFolder.TrimStart('/').TrimEnd('/');
    }
    public string AssetBundleName
    {
        get { return $"{m_outputFolder}/{Name}{Ext}"; }
    }

    public string FullName { get { return Name + Ext; } }

    public string LastHash { get; set; }
    public string CurrentHash
    {
        get { if (m_hash == null) m_hash = ComputeHash(); return m_hash; }
    }

    public bool NeedBuild { get { return CurrentHash != LastHash; } }

    public Flag BuildingFlag { get { return m_flag; } set { m_flag = value; } }

    /// <summary>
    /// º∆À„Hash
    /// </summary>
    public void ComputeHashIfNeed()
    {
        if (string.IsNullOrEmpty(m_hash))
            m_hash = ComputeHash();
    }


    #region absract
    protected abstract string ComputeHash();

    public abstract string Name { get; }

    public abstract string[] AssetNames { get; }
    #endregion

    #region virtual
    public virtual void Dispose() { }
    public virtual string Ext { get { return ".u"; } }
    #endregion

    public enum Flag { NoChange, NewAdded, Modified }
}
