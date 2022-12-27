using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IBuild
{
    void build(BuildTarget target);
}
