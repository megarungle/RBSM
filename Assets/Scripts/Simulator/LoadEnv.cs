using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadEnv : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject cylinderPrefab;
    public GameObject objects;
    public GameObject robot;
    public Camera cam;
    [SerializeField] private SelectPath uiController;

    public Material blueColor;
    public Material redColor;
    public Material blackColor;
    public Material whiteColor;
    public Material yellowColor;
    public Material greenColor;

    private int imgResolution = 256;
    private Dictionary<string, Material> colorNames;
    private Dictionary<string, GameObject> objectNames;

    private bool envLoaded = false;
    private bool robotLoaded = false;

    void Start()
    {
        colorNames = new Dictionary<string, Material>
        {
            {"blue", blueColor},
            {"red", redColor},
            {"white", whiteColor},
            {"green", greenColor},
            {"yellow", yellowColor},
            {"black", blackColor}
        };
        objectNames = new Dictionary<string, GameObject>
        {
            {"Cube(Clone)", cubePrefab},
            {"Sphere(Clone)", spherePrefab},
            {"Cylinder(Clone)", cylinderPrefab}
        };
    }

    void Update()
    {
        if (!uiController.start)
        {
            return;
        }

        if (!envLoaded)
        {
            string envName = uiController.pathEnv.Substring(uiController.pathEnv.LastIndexOf(@"\") + 1);
            DeserializeEnv(uiController.pathEnv, envName);
            envLoaded = true;
        }
        else if (!robotLoaded)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
            Vector3 direction = mousePos - cam.transform.position;

            RaycastHit hit;

            if (Input.GetMouseButtonUp(0)) // LMB to spawn the new object
            {
                if (Physics.Raycast(cam.transform.position, direction, out hit, 100.0f, 1 << 8)) // Workspace layer
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.green, 0.5f);

                    DeserializeRobot(uiController.pathRobot, hit.point);
                    robotLoaded = true;
                }
                else
                {
                    Debug.DrawLine(cam.transform.position, mousePos, Color.red, 0.5f);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                robot.transform.Rotate(0.0f, -15.0f, 0.0f, Space.World);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                robot.transform.Rotate(0.0f, 15.0f, 0.0f, Space.World);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                string scriptName = uiController.pathScript.Substring(uiController.pathScript.LastIndexOf(@"\") + 1);
                scriptName = scriptName.Substring(0, scriptName.Length - 3);
                robot.AddComponent(System.Type.GetType(scriptName + ",Assembly-CSharp"));
                Time.timeScale = 7.0f;
                cam.GetComponent<CameraRotation>().sensHor /= Time.timeScale;
                cam.GetComponent<CameraRotation>().sensVert /= Time.timeScale;
                cam.GetComponent<CameraMove>().speed /= Time.timeScale;
                Rigidbody rb = robot.AddComponent(typeof(Rigidbody)) as Rigidbody;
                rb.mass = 200;
                uiController.start = false;
            }
        }
    }

    public void DeserializeEnv(string path, string fileName)
    {
        string json = File.ReadAllText(path);
        ObjectState[] objectsParams;
        objectsParams = JsonHelper.FromJson<ObjectState>(json);

        for (int i = 0; i < objectsParams.Length; i++)
        {
            ObjectState obj = objectsParams[i];
            string name = obj.name;
            Vector3 position = obj.position;
            Quaternion rotation = obj.rotation;
            string col = obj.color;
            Vector3 s = obj.size;
            GameObject objectInst = Instantiate(objectNames[name], new Vector3(0, 0, 0), rotation, objects.transform);
            objectInst.transform.localPosition = position;
            objectInst.transform.localScale = s;
            objectInst.GetComponent<MeshRenderer>().material = colorNames[col];
            objectInst.AddComponent<Rigidbody>();
        }

        path = Application.dataPath + "/CustomFields/" + fileName.Substring(0, fileName.Length - 4) + "png";
        WWW www_tex = new WWW("file:///" + path);
        Texture2D tex = www_tex.texture;
        float localScaleY = tex.width / imgResolution;
        float localScaleX = tex.height / imgResolution;
        gameObject.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        Material fieldMaterial = renderer.material;
        fieldMaterial.mainTexture = tex;
    }

    private string ParseResource(string moduleName)
    {
        moduleName = moduleName.Substring(0, moduleName.Length - 7); // discarding "(Clone)"
        if (moduleName.Contains("Liftarm"))
        {
            return "/Balks/" + moduleName;
        }
        else if (moduleName.Contains("Pin") || moduleName == "Axle3Stub")
        {
            return "/Connectors/" + moduleName;
        }
        else if (moduleName.Contains("NXT") || moduleName.Contains("Motor") || moduleName.Contains("Sensor"))
        {
            return "/FuncElems/" + moduleName;
        }
        else if (moduleName.Contains("Axle"))
        {
            return "/Axles/" + moduleName;
        }
        else if (moduleName.Contains("cross") || moduleName.Contains("hole"))
        {
            return "/Wheels/" + moduleName;
        }
        else return "";
    }

    private IEnumerator SetSlot(GameObject module, string slot)
    {
        yield return new WaitForEndOfFrame();
        module.GetComponent<BindingFE>().slot = slot;
    }

    private void DeserializeRobot(string path, Vector3 point)
    {
        robot.transform.position = new Vector3(point.x, 0, point.z);

        string json = File.ReadAllText(path);
        ModuleState[] modulesParams;
        modulesParams = JsonHelper.FromJson<ModuleState>(json);

        float minPos = 0.0f;
        float minX = 0.0f;
        float maxX = 0.0f;
        float minZ = 0.0f;
        float maxZ = 0.0f;

        int newLen = modulesParams.Length;

        int whTransformCount = 1;
        int whColliderCount = 1;

        for (int i = 0; i < modulesParams.Length; i++)
        {
            string name = modulesParams[i].name;
            Vector3 position = modulesParams[i].position;
            Quaternion rotation = modulesParams[i].rotation;
            string slot = modulesParams[i].slot;
            GameObject newModule = Instantiate(Resources.Load("Prefabs models" + ParseResource(name)), position, rotation) as GameObject;

            if (!name.Contains("cross") && !name.Contains("hole"))
            {
                newModule.transform.SetParent(robot.transform);
            }
            else
            {
                GameObject empty = Instantiate(Resources.Load("Empty"), position, Quaternion.Euler(0, 0, 0)) as GameObject;
                empty.transform.SetParent(robot.transform);
                newModule.transform.SetParent(empty.transform);
                empty.name = newModule.name;
                newModule = empty;
            }
            newModule.transform.localPosition = position;
            if (slot != "")
            {
                StartCoroutine(SetSlot(newModule, slot));
            }

            MeshRenderer mr = newModule.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            if (mr == null) // If object has empty parent
            {
                if (!name.Contains("cross") && !name.Contains("hole"))
                {
                    mr = newModule.transform.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                    Destroy(newModule.transform.GetChild(0).gameObject.GetComponent(typeof(MeshCollider)));
                    newModule.transform.GetChild(0).gameObject.AddComponent(typeof(BoxCollider));
                    mr = newModule.transform.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                }
                else
                {
                    MeshCollider mc = newModule.transform.GetChild(0).gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                    mc.convex = true;
                    mr = newModule.transform.GetChild(0).GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                }
            }
            else
            {
                newModule.AddComponent(typeof(BoxCollider));
            }

            if (name.Contains("cross") || name.Contains("hole"))
            {
                GameObject wCol = Instantiate(Resources.Load("WCollider"), position, Quaternion.Euler(0, 0, 0), robot.transform) as GameObject;
                wCol.transform.localPosition = position;
                wCol.transform.position = new Vector3(wCol.transform.position.x, wCol.transform.position.y + 0.01f, wCol.transform.position.z);
                newLen++;

                newModule.name = newModule.name + whTransformCount.ToString();
                wCol.name = wCol.name + whColliderCount.ToString();

                whTransformCount++;
                whColliderCount++;
            }

            float pos = newModule.transform.localPosition.y - mr.bounds.size.y / 2;
            minPos = (minPos < pos) ? minPos : pos;

            float _minX = newModule.transform.localPosition.x - mr.bounds.size.x / 2;
            float _maxX = newModule.transform.localPosition.x + mr.bounds.size.x / 2;
            float _minZ = newModule.transform.localPosition.z - mr.bounds.size.z / 2;
            float _maxZ = newModule.transform.localPosition.z + mr.bounds.size.z / 2;

            minX = (minX < _minX) ? minX : _minX;
            maxX = (maxX > _maxX) ? maxX : _maxX;
            minZ = (minZ < _minZ) ? minZ : _minZ;
            maxZ = (maxZ > _maxZ) ? maxZ : _maxZ;
        }

        float avgX = Mathf.Abs(maxX - minX) / 2;
        float avgZ = Mathf.Abs(maxZ - minZ) / 2;

        for (int i = 0; i < newLen; i++)
        {
            GameObject module = robot.transform.GetChild(i).gameObject;
            module.transform.localPosition = new Vector3(module.transform.localPosition.x + avgX,
                module.transform.localPosition.y, module.transform.localPosition.z /*+ avgZ*/);
        }

        robot.transform.position = new Vector3(point.x, (minPos) * Mathf.Sign(minPos) + 0.1f, point.z);
    }
}
