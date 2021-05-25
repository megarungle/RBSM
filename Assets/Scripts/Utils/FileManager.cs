using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Drawing;

public class FileManager : MonoBehaviour
{
    void Start()
    {
    }


    public Texture2D GetTexture()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        Texture2D tex = new Texture2D(0, 0);
        if (path != null) {
            WWW www = new WWW("file:///" + path);
            tex = www.texture;
        }
        return tex;
    }
}
