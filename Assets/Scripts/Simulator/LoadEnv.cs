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


    public Material blueColor;
    public Material redColor;
    public Material blackColor;
    public Material whiteColor;
    public Material yellowColor;
    public Material greenColor;

    private int imgResolution = 256;
    private Dictionary<string, Material> colorNames;
    private Dictionary<string, GameObject> objectNames;
    bool start = true;
    
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
        if (start) {
            DeserializeEnv("C:\\Users\\Anton\\Desktop\\RBSM\\Assets\\MatsJson", "test.json"); // Path to env.json
            DeserializeRobot("C:\\Users\\Anton\\Desktop\\RBSM\\Assets\\RobotsJson", "Test.json");
            start = false;
        }
    }


    public void DeserializeEnv(string path, string fileName)
    {
        string json = File.ReadAllText(path + '\\' + fileName);
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
    
    private string parseResource(string moduleName)
    {
        moduleName = moduleName.Substring(0, moduleName.Length - 7); // discarding "(Clone)"
        if (moduleName.Contains("Liftarm"))
        {
            return "/Balks/" + moduleName;
        } else if (moduleName.Contains("Pin") || moduleName == "Axle3Stub")
        {
            return "/Connectors/" + moduleName;
        } else if (moduleName.Contains("NXT") || moduleName.Contains("Motor") || moduleName.Contains("Sensor"))
        {
            return "/FuncElems/" + moduleName;
        } else if (moduleName.Contains("Axle"))
        {
            return "/Axles/" + moduleName;
        } else if (moduleName.Contains("cross") || moduleName.Contains("hole"))
        {
            return "/Wheels/" + moduleName;
        } else return "";
    }
    
    private IEnumerator SetSlot(GameObject module, string slot)
    {
        yield return new WaitForEndOfFrame();
        module.GetComponent<BindingFE>().slot = slot;
    }

    private void DeserializeRobot(string path, string fileName)
    {
        string json = File.ReadAllText(path + '/' + fileName);
        ModuleState[] modulesParams;
        modulesParams = JsonHelper.FromJson<ModuleState>(json);
        
        for (int i = 0; i < modulesParams.Length; i++)
        {
            string name = modulesParams[i].name;
            Vector3 position = modulesParams[i].position;
            Quaternion rotation = modulesParams[i].rotation;
            string slot = modulesParams[i].slot;
            GameObject newModule = Instantiate(Resources.Load("Prefabs models" + parseResource(name)), position, rotation, robot.transform) as GameObject;
            newModule.transform.localPosition = position;
            if (slot != "")
            {
                StartCoroutine(SetSlot(newModule, slot));
            }
        }
    }
}
