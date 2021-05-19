using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.IO;
using TMPro;


public class Draw : MonoBehaviour
{
    public GameObject Field;


    public Material fieldMaterial;
    public Texture2D fieldTexture;


    const float defaultMatSizeX = 1f;
    const float defaultMatSizeY = 1f;
    const int imgResolution = 1024;


    public int brushSize = 25;
    public Color currColor = Color.black;
    public int mode = 0; // Manual
    public Vector2 firstPoint;
    public Vector2 secondPoint;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer renderer = Field.GetComponent<MeshRenderer>();
        fieldMaterial = renderer.material;
        fieldTexture = new Texture2D(imgResolution, imgResolution);
        fieldMaterial.mainTexture = fieldTexture;

        firstPoint = new Vector2(-1, -1);
        secondPoint = new Vector2(-1, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
            switch (mode) {
                case 0: // Manual
                    ManualDraw();
                    break;
                case 1: // Rectangle
                    RectDraw();
                    break;
                case 2: // Ellipse
                    EllipseDraw();
                    break;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            switch (mode) {
                case 1: // Rectangle
                    RectDraw(false);
                    break;
                case 2: // Ellipse
                    EllipseDraw(false);
                    break;
                default:
                    break;
            }
        }
    }


    void ManualDraw() {
        var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(Ray, out hit)) {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                return;
            }
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= fieldTexture.width;
            pixelUV.y *= fieldTexture.height;
            int brushScaleX = (int)(brushSize * Field.transform.localScale.z / Field.transform.localScale.x);
            int brushScaleY = (int)(brushSize * Field.transform.localScale.x / Field.transform.localScale.z);

            Color[] colors = new Color[brushScaleX * brushScaleY];
            for (int i = 0; i < brushScaleX * brushScaleY; i++) {
                colors[i] = currColor;
            }
            Debug.Log(fieldTexture.width);
            Debug.Log(pixelUV);
            if ((int)pixelUV.x >= (int)(brushScaleX / 4) && (int)pixelUV.y >= (int)(brushScaleY / 4) &&
                (int)pixelUV.x <= fieldTexture.width - (int)(brushScaleX) &&
                (int)pixelUV.y <= fieldTexture.height - (int)(brushScaleY)) {
                fieldTexture.SetPixels((int)pixelUV.x, (int)pixelUV.y, brushScaleX, brushScaleY, colors);
                fieldTexture.Apply(true);
            }
        }
    }


    void RectDraw(bool isBtnDown = true) {
        if (isBtnDown && firstPoint == new Vector2(-1, -1)) {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(Ray, out hit)) {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= fieldTexture.width;
                pixelUV.y *= fieldTexture.height;
                firstPoint = pixelUV;
            }
            return;
        } else if (!isBtnDown) {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(Ray, out hit)) {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= fieldTexture.width;
                pixelUV.y *= fieldTexture.height;
                secondPoint = pixelUV;

                int brushScaleX = (int)(brushSize * Field.transform.localScale.z / Field.transform.localScale.x);
                int brushScaleY = (int)(brushSize * Field.transform.localScale.x / Field.transform.localScale.z);

                float deltaX = secondPoint.x - firstPoint.x;
                float deltaY = secondPoint.y - firstPoint.y;

                Color[] colors = new Color[brushScaleX * brushScaleY];
                for (int i = 0; i < brushScaleX * brushScaleY; i++) {
                    colors[i] = currColor;
                }

                for (int i = 0; i <= System.Math.Abs((int)(deltaX / brushScaleX)); i++) {
                    int d = deltaX <= 0 ? -i : i;
                    fieldTexture.SetPixels((int)(firstPoint.x + d * brushScaleX), (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                    fieldTexture.SetPixels((int)(secondPoint.x - d * brushScaleX), (int)secondPoint.y, brushScaleX, brushScaleY, colors);
                }

                for (int i = 0; i <= System.Math.Abs((int)(deltaY / brushScaleY)); i++) {
                    int d = deltaY <= 0 ? -i : i;
                    fieldTexture.SetPixels((int)firstPoint.x, (int)(firstPoint.y + d * brushScaleY), brushScaleX, brushScaleY, colors);
                    fieldTexture.SetPixels((int)secondPoint.x, (int)(secondPoint.y - d * brushScaleY), brushScaleX, brushScaleY, colors);
                }

                fieldTexture.SetPixels((int)firstPoint.x, (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                fieldTexture.SetPixels((int)secondPoint.x, (int)secondPoint.y, brushScaleX, brushScaleY, colors);
                fieldTexture.SetPixels((int)(firstPoint.x + deltaX), (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                fieldTexture.SetPixels((int)firstPoint.x, (int)(firstPoint.y + deltaY), brushScaleX, brushScaleY, colors);

                fieldTexture.Apply(true);

                firstPoint = new Vector2(-1, -1);
                secondPoint = new Vector2(-1, -1);
            }
        }
    }


    void EllipseDraw(bool isBtnDown = true) {
        if (isBtnDown && firstPoint == new Vector2(-1, -1)) {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(Ray, out hit)) {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= fieldTexture.width;
                pixelUV.y *= fieldTexture.height;
                firstPoint = pixelUV;
            }
            return;
        } else if (!isBtnDown) {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(Ray, out hit)) {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
                    return;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= fieldTexture.width;
                pixelUV.y *= fieldTexture.height;
                secondPoint = pixelUV;

                int brushScaleX = (int)(brushSize * Field.transform.localScale.z / Field.transform.localScale.x);
                int brushScaleY = (int)(brushSize * Field.transform.localScale.x / Field.transform.localScale.z);

                float deltaX = secondPoint.x - firstPoint.x;
                float deltaY = secondPoint.y - firstPoint.y;

                Color[] colors = new Color[brushScaleX * brushScaleY];
                for (int i = 0; i < brushScaleX * brushScaleY; i++) {
                    colors[i] = currColor;
                }

                float xRadius = System.Math.Abs(deltaX / 2);
                float yRadius = System.Math.Abs(deltaY / 2);
                float angle = 20f;
                int segments = 1000;

                for (int i = 0; i < (segments + 1); i++) {
                    float x = Mathf.Sin (Mathf.Deg2Rad * angle) * xRadius;
                    float y = Mathf.Cos (Mathf.Deg2Rad * angle) * yRadius;
                    fieldTexture.SetPixels((int)(x + firstPoint.x + deltaX / 2), (int)(y + firstPoint.y + deltaY / 2), brushScaleX, brushScaleY, colors);
                    angle += (360f / segments);
                }

                fieldTexture.Apply(true);
                firstPoint = new Vector2(-1, -1);
                secondPoint = new Vector2(-1, -1);
            }
        }
    }


    public void recalcScales() {
        fieldTexture = new Texture2D((int)(imgResolution * Field.transform.localScale.z), (int)(imgResolution * Field.transform.localScale.x));
        fieldMaterial.mainTexture = fieldTexture;
    }

    // UI handlers
    public void ChangeMatSizeX(string newXSize) {
        if (newXSize != "") {
            float x = float.Parse(newXSize, CultureInfo.InvariantCulture);
            if (x >= 0.5 && x <= 4.95) {
                Field.transform.localScale = new Vector3(Field.transform.localScale.x, Field.transform.localScale.y, x);
            }
        } else {
            Field.transform.localScale = new Vector3(Field.transform.localScale.x, Field.transform.localScale.y, defaultMatSizeX);
        }
        recalcScales();
    }


    public void ChangeMatSizeY(string newYSize) {
        if (newYSize != "") {
            float y = float.Parse(newYSize, CultureInfo.InvariantCulture);
            if (y >= 0.5 && y <= 2.05) {
                Field.transform.localScale = new Vector3(y, Field.transform.localScale.y, Field.transform.localScale.z);
            }
        } else {
            Field.transform.localScale = new Vector3(defaultMatSizeY, Field.transform.localScale.y, Field.transform.localScale.z);
        }
        recalcScales();
    }


    public void ChangeBrushSize(Slider slider) {
        brushSize = (int)slider.value;
    }


    public void ChangeColor(Dropdown dropdown) {
        switch (dropdown.value) {
            case 0:
                currColor = Color.black;
                break;
            case 1:
                currColor = Color.white;
                break;
            case 2:
                currColor = Color.red;
                break;
            case 3:
                currColor = Color.green;
                break;
            case 4:
                currColor = Color.yellow;
                break;
            case 5:
                currColor = Color.blue;
                break;
        }
    }


    public void ResetField() {
        recalcScales();
    }


    public void SaveField(TMP_InputField fileName) {
        var data = fieldTexture.EncodeToPNG();
        string fName = fileName.text == "" ? "image" : fileName.text;
        File.WriteAllBytes(Application.dataPath + "/Images/CustomFields/" + fName + ".png", data);
    }


    public void ChangeMode(Dropdown dropdown) {
        mode = dropdown.value;
    }
}
