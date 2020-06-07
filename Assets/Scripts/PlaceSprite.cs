using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSprite : MonoBehaviour
{
    public GameObject obj;

    public void Place()
    {
        int width = GetComponent<FileManager>().GetSizes()[0];
        int height = GetComponent<FileManager>().GetSizes()[1];

        obj.GetComponent<SpriteRenderer>().sprite = GetComponent<FileManager>().GetSprite();
        obj.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        obj.GetComponent<BoxCollider>().center = new Vector3((float)width / (2 * 100), (float)height / (2 * 100), 0);
        obj.GetComponent<BoxCollider>().size = new Vector3((float)width / 100, (float)height / 100, 0);
    }
}
