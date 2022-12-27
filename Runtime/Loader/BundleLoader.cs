using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleLoader
{
    private const int DEFALUT_POOL_SIZE = 8;

    private string m_bundleName = null;
    private Dictionary<string,AssetBundle> m_bundlePool = null;

    private BundleLoader() { }
    public BundleLoader(string bundleName)
    {
        this.m_bundleName = bundleName;
        m_bundlePool = new Dictionary<string, AssetBundle>(DEFALUT_POOL_SIZE);
    }

    public AssetBundle LoadSync()
    {
        if(m_bundlePool.ContainsKey(m_bundleName))
            return m_bundlePool[m_bundleName];

        if(ResManager.AllBundlePool.ContainsKey(m_bundleName))
            return ResManager.AllBundlePool[m_bundleName];

        return null;
    }
    public void LoadAsync(Action<AssetBundle> loaded)
    {

    }

}
