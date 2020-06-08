using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSprite : MonoBehaviour
{
    public GameObject mat;
    public GameObject leftPanel;

    void Start()
    {
        leftPanel.SetActive(false);
    }

    public void Place()
    {
        int width = GetComponent<FileManager>().GetSizes()[0];
        int height = GetComponent<FileManager>().GetSizes()[1];

        mat.GetComponent<SpriteRenderer>().sprite = GetComponent<FileManager>().GetSprite();
        mat.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        mat.GetComponent<BoxCollider>().center = new Vector3((float)width / (2 * 100), (float)height / (2 * 100), 0);
        mat.GetComponent<BoxCollider>().size = new Vector3((float)width / 100, (float)height / 100, 0);

        leftPanel.SetActive(true);
    }
}
