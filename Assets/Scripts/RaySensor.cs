using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class RaySensor : MonoBehaviour
{
    public Color resColor;

    float len = 1.6f; // Ray length
    Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {
        tex = (Texture2D)Resources.Load("MatRingRes");
        resColor = Color.white;
    }

    IEnumerator ReadPixelColor(float x, float z)
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        Color[,] color = new Color[7, 7];
        float avgR = 0;
        float avgG = 0;
        float avgB = 0;

        // Square area scan
        for (int i = -3; i <= 3; ++i)
        {
            for (int j = -3; j <= 3; ++j)
            {
                color[i + 3, j + 3] = tex.GetPixel((int)(x * 100) + i, (int)(z * 100) + j);
                avgR += (float)(color[i + 3, j + 3])[0];
                avgG += (float)(color[i + 3, j + 3])[1];
                avgB += (float)(color[i + 3, j + 3])[2];
            }
        }

        avgR /= 49.0f;
        avgG /= 49.0f;
        avgB /= 49.0f;
        resColor[0] = avgR;
        resColor[1] = avgG;
        resColor[2] = avgB;
    }

    // Update is called once per frame
    void Update()
    {
        float cX = this.transform.position.x;
        float cY = this.transform.position.y - this.transform.localScale.y / 2;
        float cZ = this.transform.position.z;

        Vector3 centre = new Vector3(cX, cY, cZ); // Center of bottom edge of cube;
        Vector3 end = this.transform.TransformDirection(-Vector3.up) * len; // End point

        Debug.DrawRay(centre, end, Color.green);

        Ray r = new Ray(centre, end);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(r, out hit, len))
        {
            StartCoroutine(ReadPixelColor(hit.point.x, hit.point.z));
        }
    }
}
