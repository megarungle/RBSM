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

    public GameObject cube;
    public GameObject cylinder;
    public GameObject sphere;

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

                    Vector3 pos = new Vector3(hit.point.x, hit.point.y + height / 2, hit.point.z);

                    Instantiate(obj, pos, Quaternion.Euler(Vector3.zero));

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
