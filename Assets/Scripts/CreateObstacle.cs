using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CreateObstacle : MonoBehaviour
{
    private bool canCreate;
    private Camera cam;
    private GameObject lastObj;

    public GameObject cube;
    public GameObject cylinder;
    public GameObject sphere;

    public Material blue;
    public Material red;
    public Material white;

    private enum Obstacles
    {
        Cube = 0,
        Cylinder = 1,
        Sphere = 2
    }

    private Obstacles obs;

    public void Create(int id)
    {
        obs = (Obstacles)id;
        canCreate = true;
    }

    void Start()
    {
        canCreate = false;
        cam = this.GetComponent<Camera>();
    }

    public void UpdateSize(float size)
    {
        float prevSize = lastObj.transform.localScale.y;

        float x = lastObj.transform.position.x;
        float y = lastObj.transform.position.y;
        float z = lastObj.transform.position.z;

        Bounds b = lastObj.GetComponent<MeshFilter>().sharedMesh.bounds;

        lastObj.transform.position = new Vector3(x, y - (b.size.y * prevSize / 2) + (b.size.y * size / 2), z);
        lastObj.transform.localScale = new Vector3(size, size, size);
    }

    public void UpdateRotation(float rotation)
    {
        float x = lastObj.transform.rotation.x;
        float z = lastObj.transform.rotation.z;

        lastObj.transform.rotation = Quaternion.Euler(new Vector3(x, rotation, z));
    }

    public void UpdateColor(int color)
    {
        Material m = null;
        
        switch (color)
        {
            case 0:
                m = white;
                break;
            case 1:
                m = blue;
                break;
            case 2:
                m = red;
                break;
        }

        lastObj.GetComponent<MeshRenderer>().material = m;
    }

    void FixedUpdate()
    {
        if (canCreate)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
                Vector3 direction = mousePos - cam.transform.position;

                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f))
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                    GameObject obj = cube;
                    switch (obs)
                    {
                        case Obstacles.Cube:
                            obj = cube;
                            break;
                        case Obstacles.Cylinder:
                            obj = cylinder;
                            break;
                        case Obstacles.Sphere:
                            obj = sphere;
                            break;
                    }

                    Bounds b = obj.GetComponent<MeshFilter>().sharedMesh.bounds;
                    float height = b.size.y;

                    Vector3 pos = new Vector3(hit.point.x, hit.point.y + (height / 2.0f), hit.point.z);

                    lastObj = Instantiate(obj, pos, Quaternion.Euler(Vector3.zero));

                    canCreate = false;
                }
                else
                {
                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
                Vector3 direction = mousePos - cam.transform.position;

                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f))
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                }
            }
        }
    }
}
