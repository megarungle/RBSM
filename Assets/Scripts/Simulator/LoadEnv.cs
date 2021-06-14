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

    public Material blueColor;
    public Material redColor;
    public Material blackColor;
    public Material whiteColor;
    public Material yellowColor;
    public Material greenColor;

    private int imgResolution = 256;
    private Dictionary<string, Material> colorNames;
    private Dictionary<string, GameObject> objectNames;
    
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
        DeserializeEnv("C:\\Users\\Anton\\Desktop\\RBSM\\Assets\\MatsJson", "test.json"); // Path to env.json
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
}
