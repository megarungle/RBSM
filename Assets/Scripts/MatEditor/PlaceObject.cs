using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject Field;
    public GameObject CubePrefab;
    public GameObject SpherePrefab;
    public GameObject CylinderPrefab;

    public Material blueColor;
    public Material redColor;
    public Material blackColor;
    public Material whiteColor;
    public Material yellowColor;
    public Material greenColor;

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


    void Start()
    {
        MeshRenderer renderer = Field.GetComponent<MeshRenderer>();
        fieldMaterial = renderer.material;
        currentObject = CubePrefab;
        color = whiteColor;
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
                lastObject = Instantiate(currentObject, hit.point + new Vector3(0, (height / 2.0f), 0), Quaternion.Euler(Vector3.zero));
                UpdateScale();
                UpdateColor();
                UpdateRotation();
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green, 10f);
            }
        }
    }

    
    void UpdateScale() {
        float prevSize = lastObject.transform.localScale.y;

        float x = lastObject.transform.position.x;
        float y = lastObject.transform.position.y;
        float z = lastObject.transform.position.z;

        Bounds b = lastObject.GetComponent<MeshFilter>().sharedMesh.bounds;

        lastObject.transform.position = new Vector3(x, y - (b.size.y * prevSize / 2) + (b.size.y * size / 2), z);
        lastObject.transform.localScale = new Vector3(size, size, size);
    }


    void UpdateColor() {
        lastObject.GetComponent<MeshRenderer>().material = color;
    }


    public void UpdateRotation()
    {
        float x = lastObject.transform.rotation.x;
        float z = lastObject.transform.rotation.z;

        lastObject.transform.rotation = Quaternion.Euler(new Vector3(x, rotation, z));
    }

    // UI handlers
    public void SetField() {
        Texture2D tex = GetComponent<FileManager>().GetTexture();
        if (tex.width == 8) {
            return;
        }
        localScaleY = tex.width / imgResolution;
        localScaleX = tex.height / imgResolution;
        Field.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
        fieldMaterial.mainTexture = tex;
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
}
