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
    private MeshRenderer mr;
    private bool         canLift;
    private bool         canMove;
    private Vector3      objectPos;
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
        }

        objectName = null;
        category = null;
        path = null;

        newObject = null;
        hitEuler = Vector3.one;
        mr = null;
        objectPos = Vector3.zero;

        canLift = true;
        canMove = true;
    }

    // Round vector to chunk (0.1, 0.1, 0.1)
    private Vector3 RoundVector(Vector3 vec)
    {
        for (int i = 0; i < 3; ++i)
        {
            int tmp = (int)Mathf.Round(vec[i] * 10.0f);
            vec[i] = (float)tmp / 10.0f;
        }

        return vec;
    }

    void Start()
    {
        ResetWorkspace();

        canLift = false; // Block lifting the empty object

        cam = gameObject.GetComponent<Camera>();

        lockAxis.Add("(0.0, 0.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxis.Add("(0.0, 180.0, 0.0)", new Vector3(1, 0, 0));
        lockAxis.Add("(0.0, 0.0, 270.0)", new Vector3(0, 1, 0));

        lockAxis.Add("(0.0, 0.0, 90.0)", new Vector3(0, -1, 0));
        //
        lockAxis.Add("(0.0, 180.0, 270.0)", new Vector3(0, 1, 0));

        startOffset.Add("(0.0, 0.0, 0.0)", new Vector2(1, 0));
        startOffset.Add("(0.0, 180.0, 0.0)", new Vector2(-1, 0));
        startOffset.Add("(0.0, 0.0, 270.0)", new Vector2(0, -1));

        startOffset.Add("(0.0, 0.0, 90.0)", new Vector2(0, 1));
        //
        startOffset.Add("(0.0, 180.0, 270.0)", new Vector2(0, -1));

        reversed.Add("(0.0, 0.0, 0.0)", new Vector3(0, 180, 0));
        reversed.Add("(0.0, 180.0, 0.0)", new Vector3(0, 0, 0));
        reversed.Add("(0.0, 0.0, 270.0)", new Vector3(0, 0, 90));
        reversed.Add("(0.0, 0.0, 90.0)", new Vector3(0, 0, 270));
        //
        reversed.Add("(0.0, 180.0, 270.0)", new Vector3(0, 0, 90));
    }

    void LiftObject()
    {
        if (canLift)
        {
            if (Input.GetKey(KeyCode.W)) // Up
            {
                Vector3 pos = new Vector3(robot.transform.position.x, Mathf.Clamp(robot.transform.position.y + 0.1f, -Mathf.Infinity, 15.0f), robot.transform.position.z);
                robot.transform.position = RoundVector(pos);
            }
            else if (Input.GetKey(KeyCode.S)) // Down
            {
                Vector3 pos = new Vector3(robot.transform.position.x, Mathf.Clamp(robot.transform.position.y - 0.1f, 5.2f, Mathf.Infinity), robot.transform.position.z);
                robot.transform.position = RoundVector(pos);
            }
        }
    }

    void Update()
    {
        LiftObject();

        // objectName == null if the element was not selected on the left panel (UI)
        if (objectName != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
            Vector3 direction = mousePos - cam.transform.position;

            RaycastHit hit;

            switch (category)
            {
                case "FuncElems":
                case "Balks":
                    {
                        int layerMask = 1 << 8; // Workspace layer

                        if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn the new object
                        {
                            if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                            {
                                if (EventSystem.current.IsPointerOverGameObject()) // Ignore button click if mouse is on UI
                                {
                                    return;
                                }

                                Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                                newObject = Instantiate(Resources.Load(path), new Vector3(-500, -500, -500), Quaternion.Euler(Vector3.zero)) as GameObject;

                                mr = newObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;

                                if (mr == null) // If object has empty parent
                                {
                                    mr = newObject.transform.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                                }

                                //Debug.Log("Distance to bottom: " + Vector3.Distance(mr.bounds.center, new Vector3(mr.bounds.center.x, mr.bounds.center.y - mr.bounds.)));

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
                                objectPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                objectPos.y = objectPos.y + mr.bounds.size.y / 2;
                                newObject.transform.position = RoundVector(objectPos);
                            }
                            else
                            {
                                Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                            }
                        }

                        break;
                    }
                case "Connectors":
                    {
                        if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn the new object
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

                                objectPos = hit.transform.position;
                                objectPos.x = objectPos.x + startOffset[hitEuler.ToString()].x * (mr.bounds.size.x + 0.2f);
                                objectPos.y = objectPos.y + startOffset[hitEuler.ToString()].y * (mr.bounds.size.y + 0.2f);

                                newObject.transform.position = RoundVector(objectPos);

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
                            if (canMove)
                            {
                                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow))
                                {
                                    Vector3 newPos = new Vector3(
                                        newObject.transform.position.x + 0.1f * lockAxis[hitEuler.ToString()].x,
                                        newObject.transform.position.y + 0.1f * lockAxis[hitEuler.ToString()].y,
                                        newObject.transform.position.z + 0.1f * lockAxis[hitEuler.ToString()].z
                                        );
                                    newObject.transform.position = RoundVector(newPos);
                                    canMove = false;
                                }
                                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
                                {
                                    Vector3 newPos = new Vector3(
                                        newObject.transform.position.x - 0.1f * lockAxis[hitEuler.ToString()].x,
                                        newObject.transform.position.y - 0.1f * lockAxis[hitEuler.ToString()].y,
                                        newObject.transform.position.z - 0.1f * lockAxis[hitEuler.ToString()].z
                                        );
                                    newObject.transform.position = RoundVector(newPos);
                                    canMove = false;
                                }
                                else if (Input.GetKey(KeyCode.R))
                                {
                                    Vector3 tmpEuler = newObject.transform.rotation.eulerAngles;
                                    Debug.Log(tmpEuler);
                                    Destroy(newObject);
                                    newObject = Instantiate(Resources.Load(path), RoundVector(objectPos), Quaternion.Euler(reversed[tmpEuler.ToString()])) as GameObject;
                                    canMove = false;
                                }
                            }
                   
                            if (Input.GetKeyUp(KeyCode.UpArrow)   || Input.GetKeyUp(KeyCode.DownArrow)  ||
                                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) ||
                                Input.GetKeyUp(KeyCode.R) )
                            {
                                canMove = true;
                            }
                        }

                        break;
                    }
                default:
                    break;
            }
        }
    }
}
