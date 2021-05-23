using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBuilder : MonoBehaviour
{
    // Object vars
    private string objectName;
    private string category;
    private GameObject newObject;

    // Raycasting var
    private Camera cam;

    public void SetNames(string objName, string parentName)
    {
        objectName = objName;
        category = parentName;
    }

    private void resetWorkspace()
    {
        objectName = null;
        category = null;
        newObject = null;
    }

    void Start()
    {
        objectName = null;
        newObject = null;
        cam = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        // objectName == null if the element was not selected on the left panel (UI)
        if (objectName != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
            Vector3 direction = mousePos - cam.transform.position;

            RaycastHit hit;

            if (category != "Connectors")
            {
                int layerMask = 1 << 8; // Workspace layer

                if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn object
                {
                    if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                    {
                        Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                        Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        string path = "Prefabs models/" + category + "/" + objectName;
                        newObject = Instantiate(Resources.Load(path), pos, Quaternion.Euler(Vector3.zero)) as GameObject;
                    }
                    else
                    {
                        Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && newObject != null) // LBM to place spawned object
                {
                    resetWorkspace();
                    return;
                }
                else if (newObject != null) // Placing the spawned object
                {
                    if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                    {
                        Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        newObject.transform.position = pos;
                    }
                    else
                    {
                        Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                    }
                }
            }
            else // category == Connectors
            {
                //
            }
        }
    }
}
