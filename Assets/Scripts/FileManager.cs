using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Drawing;

public class FileManager : MonoBehaviour
{
    private string path;
    private Sprite sprite;
    private int width;
    private int height;

    public Image image;
    public GameObject previewPanel;

    void Start()
    {
        previewPanel.SetActive(false);
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public int[] GetSizes()
    {
        return new int[] { width, height };
    }

    void UpdateImage()
    {
        previewPanel.SetActive(true);

        WWW www = new WWW("file:///" + path);
        Texture2D tmp = www.texture;

        width = tmp.width;
        height = tmp.height;
        sprite = Sprite.Create(tmp, new Rect(0, 0, width, height), Vector2.zero);

        if (tmp.width > tmp.height)
        {
            Texture2D flipped = new Texture2D(tmp.height, tmp.width);
            for (int i = 0; i < tmp.height; ++i)
            {
                for (int j = 0; j < tmp.width; ++j)
                {
                    flipped.SetPixel(i, j, tmp.GetPixel(j, i));
                }
            }
            flipped.Apply();

            tmp = flipped;
        }

        image.sprite = Sprite.Create(tmp, new Rect(0, 0, tmp.width, tmp.height), Vector2.zero);
    }

    void GetImage()
    {
        if (path != null)
        {
            UpdateImage();
        }
    }

    public void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        GetImage();
    }
}
