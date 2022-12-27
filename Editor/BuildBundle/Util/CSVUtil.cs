using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVUtil
{
    public static string[][] Parse(string filePath)
    {
        return Parse(File.ReadAllText(filePath));
    }
    public static string[][] Parse(string[] lines)
    {
        string[][] result = new string[lines.Length][];
        for (int i = 0,iMax = lines.Length; i < iMax; i++)
        {
            string[] temp = lines[i].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            result[i] = temp;
        }

        return result;
    }
}
