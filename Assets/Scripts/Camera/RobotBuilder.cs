using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading;
using System.IO;


public class RobotBuilder : MonoBehaviour
{
    public GameObject robot;

    [SerializeField]
    private CanvasController canvasController;

    // Object vars
    private string       objectName;
    private string       category;
    private GameObject   newObject;
    private Vector3      hitEuler;
    private MeshRenderer mr;
    private bool         canLift;
    private bool         canInputKeys;
    private Vector3      objectPos;
    private string       path;
    private float        speed;
    private string       slot;

    // Raycasting vars
    private Camera cam;
    private int    layerMask;

    // Pins and axles positioning
    private Dictionary<string, Vector3> lockAxis = new Dictionary<string, Vector3>(24);
    private Dictionary<string, Vector3> startOffset = new Dictionary<string, Vector3>(24);
    private Dictionary<string, Vector3> reversed = new Dictionary<string, Vector3>(24);
    // Func elems and balks positioning
    private Dictionary<string, Vector3> lockAxisRL = new Dictionary<string, Vector3>(24);
    private Dictionary<string, Vector3> lockAxisUD = new Dictionary<string, Vector3>(24);

    // Slots for binding func elems
    private List<string> slots = new List<string> { "1", "2", "3", "4", "A", "B", "C" };

    // Stack of created objects
    private Stack<GameObject> stackObjects = new Stack<GameObject>(100);

    public void SetNames(string objName, string parentName)
    {
        objectName = objName;
        category = parentName;
        path = "Prefabs models/" + category + "/" + objectName;
    }

    public void SetSlot(string s)
    {
        slot = s;
    }

    private void ResetWorkspace()
    {
        if (newObject != null)
        {
            stackObjects.Push(newObject);
            newObject.transform.SetParent(robot.transform); // Attach created object to robot
        }

        objectName = null;
        category = null;
        path = null;

        layerMask = 0;

        newObject = null;
        hitEuler = Vector3.one;
        mr = null;
        objectPos = Vector3.zero;
        speed = 0.1f;
        slot = null;

        canLift = true;
        canInputKeys = true;
    }

    // Round vector to chunk (0.02, 0.02, 0.02)
    private Vector3 RoundVector(Vector3 vec)
    {
        for (int i = 0; i < 3; ++i)
        {
            vec[i] = Mathf.Round(vec[i] * 100 * 2) / (100 * 2);
        }

        return vec;
    }

    // Round vector to chunk (0.1, 0.1, 0.1)
    private Vector3 RoundVectorInit(Vector3 vec)
    {
        for (int i = 0; i < 3; ++i)
        {
            vec[i] = (float)Math.Round(vec[i], 1, MidpointRounding.AwayFromZero);
        }

        return vec;
    }

    private IEnumerator WaitSelectSlot(GameObject nObj)
    {
        canvasController.OpenPopUp(slots);

        yield return new WaitUntil(() => slot != null);

        nObj.GetComponent<BindingFE>().slot = slot;
        canvasController.ClosePopUp();
        slots.Remove(slot);
        slot = null;
    }

    private void LiftObject()
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

    void Start()
    {
        ResetWorkspace();
        if (!Directory.Exists(Application.dataPath + "/RobotsJson/"));
        {
            Directory.CreateDirectory(Application.dataPath + "/RobotsJson/");
        }

        canLift = false; // Block lifting the empty object
        speed   = 0.1f;

        cam = gameObject.GetComponent<Camera>();

        lockAxis.Add("(0.0, 0.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxis.Add("(90.0, 0.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxis.Add("(0.0, 180.0, 180.0)", new Vector3(-1, 0, 0));
        lockAxis.Add("(270.0, 0.0, 0.0)", new Vector3(-1, 0, 0));
        //
        lockAxis.Add("(0.0, 180.0, 0.0)", new Vector3(1, 0, 0));
        lockAxis.Add("(270.0, 180.0, 0.0)", new Vector3(1, 0, 0));
        lockAxis.Add("(0.0, 0.0, 180.0)", new Vector3(1, 0, 0));
        lockAxis.Add("(90.0, 180.0, 0.0)", new Vector3(1, 0, 0));
        //
        lockAxis.Add("(0.0, 0.0, 270.0)", new Vector3(0, 1, 0));
        lockAxis.Add("(0.0, 90.0, 270.0)", new Vector3(0, 1, 0));
        lockAxis.Add("(0.0, 180.0, 270.0)", new Vector3(0, 1, 0));
        lockAxis.Add("(0.0, 270.0, 270.0)", new Vector3(0, 1, 0));
        //
        lockAxis.Add("(0.0, 180.0, 90.0)", new Vector3(0, -1, 0));
        lockAxis.Add("(0.0, 270.0, 90.0)", new Vector3(0, -1, 0));
        lockAxis.Add("(0.0, 0.0, 90.0)", new Vector3(0, -1, 0));
        lockAxis.Add("(0.0, 90.0, 90.0)", new Vector3(0, -1, 0));
        //
        lockAxis.Add("(0.0, 270.0, 0.0)", new Vector3(0, 0, -1));
        lockAxis.Add("(270.0, 270.0, 0.0)", new Vector3(0, 0, -1));
        lockAxis.Add("(0.0, 90.0, 180.0)", new Vector3(0, 0, -1));
        lockAxis.Add("(90.0, 270.0, 0.0)", new Vector3(0, 0, -1));
        //
        lockAxis.Add("(0.0, 90.0, 0.0)", new Vector3(0, 0, 1));
        lockAxis.Add("(90.0, 90.0, 0.0)", new Vector3(0, 0, 1));
        lockAxis.Add("(0.0, 270.0, 180.0)", new Vector3(0, 0, 1));
        lockAxis.Add("(270.0, 90.0, 0.0)", new Vector3(0, 0, 1));

        startOffset.Add("(0.0, 0.0, 0.0)", new Vector3(1, 0, 0));
        startOffset.Add("(90.0, 0.0, 0.0)", new Vector3(1, 0, 0));
        startOffset.Add("(0.0, 180.0, 180.0)", new Vector3(1, 0, 0));
        startOffset.Add("(270.0, 0.0, 0.0)", new Vector3(1, 0, 0));
        //
        startOffset.Add("(0.0, 180.0, 0.0)", new Vector3(-1, 0, 0));
        startOffset.Add("(270.0, 180.0, 0.0)", new Vector3(-1, 0, 0));
        startOffset.Add("(0.0, 0.0, 180.0)", new Vector3(-1, 0, 0));
        startOffset.Add("(90.0, 180.0, 0.0)", new Vector3(-1, 0, 0));
        //
        startOffset.Add("(0.0, 0.0, 270.0)", new Vector3(0, -1, 0));
        startOffset.Add("(0.0, 90.0, 270.0)", new Vector3(0, -1, 0));
        startOffset.Add("(0.0, 180.0, 270.0)", new Vector3(0, -1, 0));
        startOffset.Add("(0.0, 270.0, 270.0)", new Vector3(0, -1, 0));
        //
        startOffset.Add("(0.0, 180.0, 90.0)", new Vector3(0, 1, 0));
        startOffset.Add("(0.0, 270.0, 90.0)", new Vector3(0, 1, 0));
        startOffset.Add("(0.0, 0.0, 90.0)", new Vector3(0, 1, 0));
        startOffset.Add("(0.0, 90.0, 90.0)", new Vector3(0, 1, 0));
        //
        startOffset.Add("(0.0, 270.0, 0.0)", new Vector3(0, 0, 1));
        startOffset.Add("(270.0, 270.0, 0.0)", new Vector3(0, 0, 1));
        startOffset.Add("(0.0, 90.0, 180.0)", new Vector3(0, 0, 1));
        startOffset.Add("(90.0, 270.0, 0.0)", new Vector3(0, 0, 1));
        //
        startOffset.Add("(0.0, 90.0, 0.0)", new Vector3(0, 0, -1));
        startOffset.Add("(90.0, 90.0, 0.0)", new Vector3(0, 0, -1));
        startOffset.Add("(0.0, 270.0, 180.0)", new Vector3(0, 0, -1));
        startOffset.Add("(270.0, 90.0, 0.0)", new Vector3(0, 0, -1));

        reversed.Add("(0.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        reversed.Add("(90.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        reversed.Add("(0.0, 180.0, 180.0)", new Vector3(0, 1, 0));
        reversed.Add("(270.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        //
        reversed.Add("(0.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        reversed.Add("(270.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        reversed.Add("(0.0, 0.0, 180.0)", new Vector3(0, 1, 0));
        reversed.Add("(90.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        //
        reversed.Add("(0.0, 0.0, 270.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 90.0, 270.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 180.0, 270.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 270.0, 270.0)", new Vector3(0, 0, 1));
        //
        reversed.Add("(0.0, 180.0, 90.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 270.0, 90.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 0.0, 90.0)", new Vector3(0, 0, 1));
        reversed.Add("(0.0, 90.0, 90.0)", new Vector3(0, 0, 1));
        //
        reversed.Add("(0.0, 270.0, 0.0)", new Vector3(1, 0, 0));
        reversed.Add("(270.0, 270.0, 0.0)", new Vector3(1, 0, 0));
        reversed.Add("(0.0, 90.0, 180.0)", new Vector3(1, 0, 0));
        reversed.Add("(90.0, 270.0, 0.0)", new Vector3(1, 0, 0));
        //
        reversed.Add("(0.0, 90.0, 0.0)", new Vector3(1, 0, 0));
        reversed.Add("(90.0, 90.0, 0.0)", new Vector3(1, 0, 0));
        reversed.Add("(0.0, 270.0, 180.0)", new Vector3(1, 0, 0));
        reversed.Add("(270.0, 90.0, 0.0)", new Vector3(1, 0, 0));

        //==========================================================//

        lockAxisRL.Add("(0.0, 0.0, 0.0)", new Vector3(0, 0, 1));
        lockAxisRL.Add("(90.0, 0.0, 0.0)", new Vector3(0, 0, 1));
        lockAxisRL.Add("(0.0, 180.0, 180.0)", new Vector3(0, 0, 1));
        lockAxisRL.Add("(270.0, 0.0, 0.0)", new Vector3(0, 0, 1));
        //
        lockAxisRL.Add("(0.0, 180.0, 0.0)", new Vector3(0, 0, -1));
        lockAxisRL.Add("(270.0, 180.0, 0.0)", new Vector3(0, 0, -1));
        lockAxisRL.Add("(0.0, 0.0, 180.0)", new Vector3(0, 0, -1));
        lockAxisRL.Add("(90.0, 180.0, 0.0)", new Vector3(0, 0, -1));
        //
        lockAxisRL.Add("(0.0, 0.0, 270.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 90.0, 270.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 180.0, 270.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 270.0, 270.0)", new Vector3(1, 0, 0));
        //
        lockAxisRL.Add("(0.0, 180.0, 90.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 270.0, 90.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 0.0, 90.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 90.0, 90.0)", new Vector3(1, 0, 0));
        //
        lockAxisRL.Add("(0.0, 270.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxisRL.Add("(270.0, 270.0, 0.0)", new Vector3(-1, 0, 0));
        lockAxisRL.Add("(0.0, 90.0, 180.0)", new Vector3(-1, 0, 0));
        lockAxisRL.Add("(90.0, 270.0, 0.0)", new Vector3(-1, 0, 0));
        //
        lockAxisRL.Add("(0.0, 90.0, 0.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(90.0, 90.0, 0.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(0.0, 270.0, 180.0)", new Vector3(1, 0, 0));
        lockAxisRL.Add("(270.0, 90.0, 0.0)", new Vector3(1, 0, 0));

        lockAxisUD.Add("(0.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(90.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(0.0, 180.0, 180.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(270.0, 0.0, 0.0)", new Vector3(0, 1, 0));
        //
        lockAxisUD.Add("(0.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(270.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(0.0, 0.0, 180.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(90.0, 180.0, 0.0)", new Vector3(0, 1, 0));
        //
        lockAxisUD.Add("(0.0, 0.0, 270.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 90.0, 270.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 180.0, 270.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 270.0, 270.0)", new Vector3(0, 0, 1));
        //
        lockAxisUD.Add("(0.0, 180.0, 90.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 270.0, 90.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 0.0, 90.0)", new Vector3(0, 0, 1));
        lockAxisUD.Add("(0.0, 90.0, 90.0)", new Vector3(0, 0, 1));
        //
        lockAxisUD.Add("(0.0, 270.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(270.0, 270.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(0.0, 90.0, 180.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(90.0, 270.0, 0.0)", new Vector3(0, 1, 0));
        //
        lockAxisUD.Add("(0.0, 90.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(90.0, 90.0, 0.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(0.0, 270.0, 180.0)", new Vector3(0, 1, 0));
        lockAxisUD.Add("(270.0, 90.0, 0.0)", new Vector3(0, 1, 0));
    }

    void Update()
    {
        if (canInputKeys && Input.GetKey(KeyCode.RightShift))
        {
            speed = 0.12f - speed;
            canInputKeys = false;
        }
        else if (canInputKeys && newObject == null && category == null && Input.GetKey(KeyCode.Delete))
        {
            if (stackObjects.Count != 0)
            {
                Destroy(stackObjects.Pop());
                canInputKeys = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.Delete))
        {
            canInputKeys = true;
        }

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
                case "Wheels":
                    {
                        if (objectName == "NXT")
                        {
                            layerMask = 1 << 8; // Workspace layer

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
                                // Moving
                                if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, layerMask))
                                {
                                    objectPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                                    objectPos.y = objectPos.y + mr.bounds.size.y / 2;
                                    newObject.transform.position = RoundVectorInit(objectPos);
                                }
                                else
                                {
                                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                                }

                                if (canInputKeys)
                                {
                                    // Rotation
                                    if (Input.GetKey(KeyCode.UpArrow))
                                    {
                                        newObject.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.RightArrow))
                                    {
                                        newObject.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.DownArrow))
                                    {
                                        newObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.LeftArrow))
                                    {
                                        newObject.transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                }

                                if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) ||
                                    Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                                {
                                    canInputKeys = true;
                                }
                            }
                        }
                        else // Not a NXT
                        {
                            if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn the new object
                            {
                                layerMask = 1 << 10; // Pins and axles layer

                                if (objectName == "Liftarm90Degr")
                                {
                                    layerMask |= 1 << 9;
                                }

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
                                    objectPos.x = objectPos.x + startOffset[hitEuler.ToString()].x * (mr.bounds.size.x + 0.3f);
                                    objectPos.y = objectPos.y + startOffset[hitEuler.ToString()].y * (mr.bounds.size.y + 0.3f);
                                    objectPos.z = objectPos.z + startOffset[hitEuler.ToString()].z * (mr.bounds.size.z + 0.3f);

                                    newObject.transform.position = RoundVectorInit(objectPos);

                                    canLift = false; // Block lifting the robot
                                }
                                else
                                {
                                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                                }
                            }
                            else if (Input.GetMouseButtonUp(0) && newObject != null) // LBM to place spawned object
                            {
                                if (category == "FuncElems")
                                {
                                    StartCoroutine(WaitSelectSlot(newObject));
                                }
                                ResetWorkspace();
                                return;
                            }
                            else if (newObject != null) // Placing the spawned object
                            {
                                if (canInputKeys)
                                {
                                    if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.R))
                                    {
                                        newObject.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.R))
                                    {
                                        newObject.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.R))
                                    {
                                        newObject.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.R))
                                    {
                                        newObject.transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftAlt))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x + speed * lockAxisUD[hitEuler.ToString()].x,
                                            newObject.transform.position.y + speed * lockAxisUD[hitEuler.ToString()].y,
                                            newObject.transform.position.z + speed * lockAxisUD[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftAlt))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x - speed * lockAxisUD[hitEuler.ToString()].x,
                                            newObject.transform.position.y - speed * lockAxisUD[hitEuler.ToString()].y,
                                            newObject.transform.position.z - speed * lockAxisUD[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.UpArrow))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x + speed * lockAxis[hitEuler.ToString()].x,
                                            newObject.transform.position.y + speed * lockAxis[hitEuler.ToString()].y,
                                            newObject.transform.position.z + speed * lockAxis[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.DownArrow))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x - speed * lockAxis[hitEuler.ToString()].x,
                                            newObject.transform.position.y - speed * lockAxis[hitEuler.ToString()].y,
                                            newObject.transform.position.z - speed * lockAxis[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.RightArrow))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x + speed * lockAxisRL[hitEuler.ToString()].x,
                                            newObject.transform.position.y + speed * lockAxisRL[hitEuler.ToString()].y,
                                            newObject.transform.position.z + speed * lockAxisRL[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                    else if (Input.GetKey(KeyCode.LeftArrow))
                                    {
                                        Vector3 newPos = new Vector3(
                                            newObject.transform.position.x - speed * lockAxisRL[hitEuler.ToString()].x,
                                            newObject.transform.position.y - speed * lockAxisRL[hitEuler.ToString()].y,
                                            newObject.transform.position.z - speed * lockAxisRL[hitEuler.ToString()].z
                                            );
                                        newObject.transform.position = RoundVector(newPos);
                                        canInputKeys = false;
                                    }
                                }

                                if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) ||
                                    Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) ||
                                    Input.GetKeyUp(KeyCode.R))
                                {
                                    canInputKeys = true;
                                }
                            }
                        }

                        break;
                    }
                case "Axles":
                    {
                        layerMask = 1 << 11; // Сross layer
                        goto case "Connectors";
                    }
                case "Connectors":
                    {
                        if (Input.GetMouseButtonUp(0) && newObject == null) // LMB to spawn the new object
                        {
                            layerMask |= 1 << 9; // Groove layer

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
                                objectPos.x = objectPos.x + startOffset[hitEuler.ToString()].x * (mr.bounds.size.x + 0.1f);
                                objectPos.y = objectPos.y + startOffset[hitEuler.ToString()].y * (mr.bounds.size.y + 0.1f);
                                objectPos.z = objectPos.z + startOffset[hitEuler.ToString()].z * (mr.bounds.size.z + 0.1f);

                                // Align to (0.1, 0.1, 0.1) chuck only locked axis
                                for (int i = 0; i < 3; ++i)
                                {
                                    if (lockAxis[hitEuler.ToString()][i] != 0)
                                    {
                                        objectPos[i] = (float)Math.Round(objectPos[i], 1, MidpointRounding.AwayFromZero);
                                    }
                                }

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
                            if (canInputKeys)
                            {
                                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow))
                                {
                                    Vector3 newPos = new Vector3(
                                        newObject.transform.position.x + speed * lockAxis[hitEuler.ToString()].x,
                                        newObject.transform.position.y + speed * lockAxis[hitEuler.ToString()].y,
                                        newObject.transform.position.z + speed * lockAxis[hitEuler.ToString()].z
                                        );
                                    newObject.transform.position = RoundVector(newPos);
                                    canInputKeys = false;
                                }
                                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
                                {
                                    Vector3 newPos = new Vector3(
                                        newObject.transform.position.x - speed * lockAxis[hitEuler.ToString()].x,
                                        newObject.transform.position.y - speed * lockAxis[hitEuler.ToString()].y,
                                        newObject.transform.position.z - speed * lockAxis[hitEuler.ToString()].z
                                        );
                                    newObject.transform.position = RoundVector(newPos);
                                    canInputKeys = false;
                                }
                                else if (Input.GetKey(KeyCode.R))
                                {
                                    // Reserved implementation of rotation, it is necessary to refactor reversed dict for using it 
                                    /*
                                    Vector3 tmpEuler = newObject.transform.rotation.eulerAngles;
                                    Destroy(newObject);
                                    newObject = Instantiate(Resources.Load(path), RoundVector(objectPos), Quaternion.Euler(reversed[tmpEuler.ToString()])) as GameObject;
                                    */
                                    float x = 180.0f * reversed[hitEuler.ToString()].x;
                                    float y = 180.0f * reversed[hitEuler.ToString()].y;
                                    float z = 180.0f * reversed[hitEuler.ToString()].z;
                                    newObject.transform.Rotate(x, y, z, Space.World);
                                    newObject.transform.position = RoundVector(objectPos);
                                    canInputKeys = false;
                                }
                            }
                   
                            if (Input.GetKeyUp(KeyCode.UpArrow)   || Input.GetKeyUp(KeyCode.DownArrow)  ||
                                Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) ||
                                Input.GetKeyUp(KeyCode.R) )
                            {
                                canInputKeys = true;
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
