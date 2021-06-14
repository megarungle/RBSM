using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomFileManager : EditorWindow
{
    [MenuItem("FileManager/Load")]
    static string Load(string extension, string dir)
    {
        string path = EditorUtility.SaveFilePanel("Select the file to load", Application.dataPath + dir, "", extension);
        return path;
    }

    [MenuItem("FileManager/Save")]
    static string Save(string extension, string dir)
    {
        string path = EditorUtility.OpenFilePanel("Select the folder to save", Application.dataPath + dir, extension);
        return path;
    }
}
