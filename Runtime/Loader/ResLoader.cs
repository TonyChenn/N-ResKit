using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResLoader
{
    private string m_bundleName = null;

    private ResLoader() { }
    public ResLoader(string bundleName)
    {
        this.m_bundleName = bundleName;
    }

}
