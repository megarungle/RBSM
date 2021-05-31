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
        string[] files = Directory.GetFiles(Application.dataPath + "/CustomFields/", "*." + type);
        return files;
    }
}
