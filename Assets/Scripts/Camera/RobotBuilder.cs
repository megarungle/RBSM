using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;

public class RobotBuilder : MonoBehaviour
{
    public GameObject robot;

    // Object vars
    private string       objectName;
    private string       category;
    private GameObject   newObject;
    private Vector3      hitEuler;
    private Rigidbody    rg;
    private MeshRenderer mr;
    private bool         canLift;
    private bool         canRotate;
    private Vector3      connectorPos;
    private string       path;

    // Raycasting var
    private Camera cam;

    private Dictionary<string, Vector3> lockAxis = new Dictionary<string, Vector3>(20);
    private Dictionary<string, Vector2> startOffset = new Dictionary<string, Vector2>(20);
    private Dictionary<string, Vector3> reversed = new Dictionary<string, Vector3>(20);

    public void SetNames(string objName, string parentName)
    {
        objectName = objName;
        category = parentName;
        path = "Prefabs models/" + category + "/" + objectName;
    }

    private void ResetWorkspace()
    {
        if (newObject != null)
        {
            newObject.transform.SetParent(robot.transform); // Attach created object to robot
            
            rg = newObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
            if (rg != null)
            {
                Destroy(rg);
            }
        }

        objectName = null;
        category = null;
        path = null;

        newObject = null;
        hitEuler = Vector3.one;
        rg = null;
        mr = null;
        connectorPos = Vector3.zero;

        canLift = true;
        canRotate = true;
    }

    void Start()
    {
        ResetWorkspace();

        canLift = false; // Block lifting the empty object

        cam = gameObject.GetComponent<Camera>();

        lockAxis.Add("(0.0, 0.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxis.Add("(0.0, 180.0, 0.0)", new Vector3(1, 0, 0));
        lockAxis.Add("(0.0, 0.0, 270.0)", new Vector3(0, 1, 0));
        lockAxis.Add("(0.0, 0.0, 90.0)", new Vector3());

        lockAxis.Add("(0.0, 180.0, 270.0)", new Vector3(0, 1, 0));

        startOffset.Add("(0.0, 0.0, 0.0)", new Vector2(1, 0));
        startOffset.Add("(0.0, 180.0, 0.0)", new Vector2(-1, 0));
        startOffset.Add("(0.0, 0.0, 270.0)", new Vector2(0, -1));
        startOffset.Add("(0.0, 0.0, 90.0)", new Vector2());

        startOffset.Add("(0.0, 180.0, 270.0)", new Vector2(0, -1));

        reversed.Add("(0.0, 0.0, 0.0)", new Vector3(0, 180, 0));
        reversed.Add("(0.0, 180.0, 0.0)", new Vector3(0, 0, 0));
        reversed.Add("(0.0, 0.0, 270.0)", new Vector3(0, 0, 90));
        reversed.Add("(0.0, 0.0, 90.0)", new Vector3(0, 0, 270));

        reversed.Add("(0.0, 180.0, 270.0)", new Vector3(0, 0, 90));
    }

    IEnumerator StopMoving(Rigidbody rg)
    {
        yield return new WaitForSeconds(0.01f);
        rg.velocity = Vector3.zero;
    }

    IEnumerator PauseRotation()
    {
        canRotate = false;
        yield return new WaitForSeconds(0.2f);
        canRotate = true;
    }

    void LiftObject()
    {
        if (canLift)
        {
            if (Input.GetKey(KeyCode.W)) // Up
            {
                Vector3 pos = new Vector3(robot.transform.position.x, Mathf.Clamp(robot.transform.position.y + 0.1f, -Mathf.Infinity, 15.0f), robot.transform.position.z);
                robot.transform.position = pos;
            }
            else if (Input.GetKey(KeyCode.S)) // Down
            {
                Vector3 pos = new Vector3(robot.transform.position.x, Mathf.Clamp(robot.transform.position.y - 0.1f, 5.2f, Mathf.Infinity), robot.transform.position.z);
                robot.transform.position = pos;
            }
        }
    }

    void FixedUpdate()
    {
        LiftObject();

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
                        if (EventSystem.current.IsPointerOverGameObject()) // Ignore button click if mouse is on UI
                        {
                            return;
                        }

                        Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                        Vector3 pos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        newObject = Instantiate(Resources.Load(path), pos, Quaternion.Euler(Vector3.zero)) as GameObject;

                        canLift = false; // Block lifting the robot
                    }
                    else
                    {
                        Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && newObject != null) // LBM to place spawned object
                {
                    ResetWorkspace();
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
                if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn object
                {
                    int layerMask = 1 << 9; // Groove layer

                    if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                    {
                        if (EventSystem.current.IsPointerOverGameObject()) // Ignore button click if mouse is on UI
                        {
                            return;
                        }

                        Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                        hitEuler = hit.transform.parent.rotation.eulerAngles;
                        Debug.Log(hitEuler);

                        newObject = Instantiate(Resources.Load(path), new Vector3(-500, -500, -500), Quaternion.Euler(hitEuler)) as GameObject;

                        mr = newObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;

                        connectorPos = hit.transform.position;
                        connectorPos.x = connectorPos.x + startOffset[hitEuler.ToString()].x * (mr.bounds.size.x + 0.1f);
                        connectorPos.y = connectorPos.y + startOffset[hitEuler.ToString()].y * (mr.bounds.size.y + 0.1f);

                        newObject.transform.position = connectorPos;

                        rg = newObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
                        rg.freezeRotation = true;

                        canLift = false; // Block lifting the robot
                    }
                    else
                    {
                        Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && newObject != null) // LBM to place spawned object
                {
                    rg.constraints = RigidbodyConstraints.FreezeAll; // Freeze the object
                    ResetWorkspace();
                    return;
                }
                else if (newObject != null) // Placing the spawned object
                {
                    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow))
                    {
                        rg.velocity = lockAxis[hitEuler.ToString()].normalized;
                        StartCoroutine(StopMoving(rg));
                    }
                    else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        rg.velocity = -lockAxis[hitEuler.ToString()].normalized;
                        StartCoroutine(StopMoving(rg));
                    }
                    else if (Input.GetKey(KeyCode.R))
                    {
                        if (canRotate)
                        {
                            Vector3 tmpEuler = newObject.transform.rotation.eulerAngles;
                            Destroy(newObject);
                            newObject = Instantiate(Resources.Load(path), connectorPos, Quaternion.Euler(reversed[tmpEuler.ToString()])) as GameObject;

                            rg = newObject.GetComponent(typeof(Rigidbody)) as Rigidbody;
                            rg.freezeRotation = true;

                            StartCoroutine(PauseRotation());
                        }
                    }
                }
            }
        }
    }
}
