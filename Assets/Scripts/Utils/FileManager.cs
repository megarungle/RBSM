using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Drawing;
using System.IO;
using System.Linq;
using TMPro;

public class FileManager : MonoBehaviour
{
    void Start()
    {
    }


    public string[] GetFiles(string type) 
    {
        string path = Application.dataPath + "/CustomFields/";
        string[] files = new string[0];
        if (!Directory.Exists(path))
        {
            return files;
        }
        files = Directory.GetFiles(path, "*." + type);
        return files;
    }
}
