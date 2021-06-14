using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using TMPro;


[System.Serializable]
public class ObjectState
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string color;
    public Vector3 size;

    public ObjectState(string n, Vector3 pos, Quaternion rot, string col, Vector3 s)
    {
        name = n;
        position = pos;
        rotation = rot;
        color = col;
        size = s;
    }

    public void print()
    {
        Debug.Log(name);
        Debug.Log(position);
        Debug.Log(rotation);
        Debug.Log(color);
        Debug.Log(size);
    }
}

public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}

public class PlaceObject : MonoBehaviour
{
    public GameObject Field;
    public GameObject Objects;
    public GameObject CubePrefab;
    public GameObject SpherePrefab;
    public GameObject CylinderPrefab;
    public Button SaveMatBtn;

    public Material blueColor;
    public Material redColor;
    public Material blackColor;
    public Material whiteColor;
    public Material yellowColor;
    public Material greenColor;
    
    public Canvas explorer;
    public GameObject item;
    private Canvas exp;

    private Material fieldMaterial;
    private Texture2D fieldTexture;
    private GameObject currentObject;
    private GameObject lastObject;

    private float localScaleX = 1f;
    private float localScaleY = 1f;
    private float size = 1f;
    private float rotation = 0f;
    private Material color;

    private int imgResolution = 256;
    private Dictionary<Material, string> colorNames;
    private Dictionary<string, GameObject> objectNames;
    private string fileImage = "";


    void Start()
    {
        MeshRenderer renderer = Field.GetComponent<MeshRenderer>();
        fieldMaterial = renderer.material;
        currentObject = CubePrefab;
        color = whiteColor;
        colorNames = new Dictionary<Material, string>
        {
            {blueColor, "blue"},
            {redColor, "red"},
            {whiteColor, "white"},
            {greenColor, "green"},
            {yellowColor, "yellow"},
            {blackColor, "black"}
        };
        objectNames = new Dictionary<string, GameObject>
        {
            {"Cube(Clone)", CubePrefab},
            {"Sphere(Clone)", SpherePrefab},
            {"Cylinder(Clone)", CylinderPrefab}
        };
        SaveMatBtn.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(Ray, out hit))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || hit.transform.gameObject != Field) {
                    return;
                }
                Bounds b = currentObject.GetComponent<MeshFilter>().sharedMesh.bounds;
                float height = b.size.y;
                lastObject = Instantiate(currentObject, hit.point + new Vector3(0, (height / 2.0f), 0), Quaternion.Euler(Vector3.zero), Objects.transform);
                UpdateScale();
                UpdateColor();
                UpdateRotation();
            }
        }

        if (lastObject && Input.GetKey(KeyCode.Q))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));
            RaycastHit hit;
            Vector3 direction = mousePos - Camera.main.transform.position;

            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 100.0f, 1 << 8))
            {
                Bounds b = currentObject.GetComponent<MeshFilter>().sharedMesh.bounds;
                float height = b.size.y;
                lastObject.transform.position = hit.point + new Vector3(0, (height / 2.0f) * lastObject.transform.localScale.y, 0);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && exp != null)
        {
            Destroy(exp.gameObject);
        }
    }

    
    void UpdateScale() {
        if (!lastObject)
        {
            return;
        }
        float prevSize = lastObject.transform.localScale.y;

        float x = lastObject.transform.position.x;
        float y = lastObject.transform.position.y;
        float z = lastObject.transform.position.z;

        Bounds b = lastObject.GetComponent<MeshFilter>().sharedMesh.bounds;

        lastObject.transform.position = new Vector3(x, y - (b.size.y * prevSize / 2) + (b.size.y * size / 2), z);
        lastObject.transform.localScale = new Vector3(size, size, size);
    }


    void UpdateColor() {
        if (!lastObject)
        {
            return;
        }
        lastObject.GetComponent<MeshRenderer>().material = color;
    }


    public void UpdateRotation()
    {
        if (!lastObject)
        {
            return;
        }
        float x = lastObject.transform.rotation.x;
        float z = lastObject.transform.rotation.z;

        lastObject.transform.rotation = Quaternion.Euler(new Vector3(x, rotation, z));
    }
    
    private void ShowExplorerFields(string[] files)
    {
        if (exp != null)
        {
            return;
        }
        exp = Object.Instantiate(explorer);
        Transform content = exp.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);
        for (int i = 0; i < files.Length; i++)
        {
            GameObject newItem = Object.Instantiate(item, content);
            string fileName = files[i].Split('/').Last();
            newItem.transform.GetChild(1).GetComponent<TMP_Text>().text = fileName;
            WWW www = new WWW("file:///" + files[i]);
            Sprite previewSprite = Sprite.Create(www.texture, new Rect(0.0f, 0.0f, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            Image itemImage = newItem.transform.GetChild(0).GetComponent<Image>() as Image;
            itemImage.sprite = previewSprite;
            Button btn = newItem.GetComponent<Button>();
            btn.onClick.AddListener(
                () =>
                {
                    string path = Application.dataPath + "/CustomFields/" + fileName;
                    WWW www_tex = new WWW("file:///" + path);
                    Texture2D tex = www_tex.texture;
                    localScaleY = tex.width / imgResolution;
                    localScaleX = tex.height / imgResolution;
                    Field.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
                    foreach (Transform child in Objects.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    fieldMaterial.mainTexture = tex;
                    fileImage = fileName;
                    SaveMatBtn.gameObject.SetActive(true);
                    Destroy(exp.gameObject);
                }
            );
        }
    }
    

    // UI handlers
    
    public void SetField()
    {
        string[] files = GetComponent<FileManager>().GetFiles("png");
        ShowExplorerFields(files);
    }


    public void SetSize(float newValue) {
        size = newValue;
        UpdateScale();
    }


    public void SetRotation(float newValue) {
        rotation = newValue;
        UpdateRotation();
    }


    public void SetColor(Dropdown dropdown) {
        string objectColor = dropdown.options[dropdown.value].text;
        switch (objectColor) {
            case "Black":
                color = blackColor;
                break;
            case "Red":
                color = redColor;
                break;
            case "Green":
                color = greenColor;
                break;
            case "Yellow":
                color = yellowColor;
                break;
            case "Blue":
                color = blueColor;
                break;
            case "White":
                color = whiteColor;
                break;
        }
        UpdateColor();
    }

    public void SetObject(Dropdown dropdown) {
        string objectType = dropdown.options[dropdown.value].text;
        switch (objectType) {
            case "Cube":
                currentObject = CubePrefab;
                break;
            case "Sphere":
                currentObject = SpherePrefab;
                break;
            case "Cylinder":
                currentObject = CylinderPrefab;
                break;
        }
    }
    
    
    public IEnumerator saveToDisk(string path, string data)
    {
        yield return new WaitForSeconds(2);
        File.WriteAllText(path, data);

    }
    
    public void SerializeField()
    {
        ObjectState[] objectsParams = new ObjectState[Objects.transform.childCount];
        for (int i = 0; i < Objects.transform.childCount; i++)
        {
            Transform obj = Objects.transform.GetChild(i);
            string name = obj.name;
            Vector3 position = obj.position;
            Quaternion rotation = obj.rotation;
            string col = colorNames[obj.GetComponent<MeshRenderer>().sharedMaterial];
            ObjectState state = new ObjectState(name, position, rotation, col, obj.localScale);
            objectsParams[i] = state;
        }
        string objectsToJson = JsonHelper.ToJson(objectsParams, true);
        string fileName = fileImage.Substring(0, fileImage.Length - 3) + "json";
        string path = Application.dataPath + "/MatsJson/";
        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        
        if(!File.Exists(path + fileName)) {
            File.Create(path + fileName);
        }
        
        StartCoroutine(saveToDisk(path + fileName, objectsToJson));
    }


    public void DeserializeField()
    { 
        string fileName = "Test.json"; // TODO: Need to get this filename from explorer
        string path = Application.dataPath + "/MatsJson/";
        string json = File.ReadAllText(path + fileName);
        ObjectState[] objectsParams;
        objectsParams = JsonHelper.FromJson<ObjectState>(json);
        
        foreach (Transform child in Objects.transform)
        {
            Destroy(child.gameObject);
        }
        
        Dictionary<string, Material> revertedDict = new Dictionary<string, Material>();
        foreach (Material key in colorNames.Keys)
        {
            revertedDict[colorNames[key]] = key;
        }
        
        for (int i = 0; i < objectsParams.Length; i++)
        {
            ObjectState obj = objectsParams[i];
            string name = obj.name;
            Vector3 position = obj.position;
            Quaternion rotation = obj.rotation;
            string col = obj.color;
            Vector3 s = obj.size;
            GameObject objectInst = Instantiate(objectNames[name], position, rotation, Objects.transform);
            objectInst.transform.localScale = s;
            objectInst.GetComponent<MeshRenderer>().material = revertedDict[col];
        }
        
        path = Application.dataPath + "/CustomFields/" + fileName.Substring(0, fileName.Length - 4) + "png";
        WWW www_tex = new WWW("file:///" + path);
        Texture2D tex = www_tex.texture;
        localScaleY = tex.width / imgResolution;
        localScaleX = tex.height / imgResolution;
        Field.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
        fieldMaterial.mainTexture = tex;
        fileImage = fileName.Substring(0, fileName.Length - 4) + "png";
        SaveMatBtn.gameObject.SetActive(true);
    }
}
