using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
    private string objectName;
    private Camera cam;
    int layerMask;

    public void SetObjectName(string name)
    {
        objectName = name;
    }

    void Start()
    {
        objectName = null;
        cam = gameObject.GetComponent<Camera>();
        layerMask = 1 << 8; // Workspace layer
    }

    void Update()
    {
        if (objectName != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
                Vector3 direction = mousePos - cam.transform.position;

                RaycastHit hit;
                
                if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);
                    /*
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
                    */
                }
                else
                {
                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                }
            }
        }
    }
}
