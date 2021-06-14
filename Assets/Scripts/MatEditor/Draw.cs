using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Linq;
using TMPro;
using SimpleFileBrowser;

public class Draw : MonoBehaviour
{
    public GameObject Field;

    public Canvas explorer;
    public GameObject item;
    public int brushSize = 10;
    public Color currColor = Color.black;
    public int mode = 0; // Manual
    private Vector2 firstPoint;
    private Vector2 secondPoint;
    private Texture2D savedTex;
    private Material fieldMaterial;
    private Texture2D fieldTexture;
    private Canvas exp;
    
    const float defaultMatSizeX = 1f;
    const float defaultMatSizeY = 1f;
    const int imgResolution = 256;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer renderer = Field.GetComponent<MeshRenderer>();
        fieldMaterial = renderer.material;
        recalcScales();
        fieldMaterial.mainTexture = fieldTexture;

        firstPoint = new Vector2(-1, -1);
        secondPoint = new Vector2(-1, -1);

        if (!Directory.Exists(Application.dataPath + "/CustomFields")) {
            Directory.CreateDirectory(Application.dataPath + "/CustomFields");
        }
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
                case 0: //Manual
                    ManualDraw(false);
                    break;
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

        if (Input.GetKeyDown(KeyCode.Escape) && exp != null)
        {
            Destroy(exp.gameObject);
        }
    }


    void ManualDraw(bool isBtnDown = true) {
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
            if ((int)pixelUV.x >= (int)(brushScaleX / 4) && (int)pixelUV.y >= (int)(brushScaleY / 4) &&
                (int)pixelUV.x <= fieldTexture.width - (int)(brushScaleX) &&
                (int)pixelUV.y <= fieldTexture.height - (int)(brushScaleY)) {
                fieldTexture.SetPixels((int)pixelUV.x, (int)pixelUV.y, brushScaleX, brushScaleY, colors);
                fieldTexture.Apply();
            }
        }
        if (!isBtnDown) {
            Graphics.CopyTexture(fieldTexture, savedTex);
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
        } else if (firstPoint != new Vector2(-1, -1)) {
            Graphics.CopyTexture(fieldTexture, savedTex);
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
                    savedTex.SetPixels((int)(firstPoint.x + d * brushScaleX), (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                    savedTex.SetPixels((int)(secondPoint.x - d * brushScaleX), (int)secondPoint.y, brushScaleX, brushScaleY, colors);
                }

                for (int i = 0; i <= System.Math.Abs((int)(deltaY / brushScaleY)); i++) {
                    int d = deltaY <= 0 ? -i : i;
                    savedTex.SetPixels((int)firstPoint.x, (int)(firstPoint.y + d * brushScaleY), brushScaleX, brushScaleY, colors);
                    savedTex.SetPixels((int)secondPoint.x, (int)(secondPoint.y - d * brushScaleY), brushScaleX, brushScaleY, colors);
                }

                savedTex.SetPixels((int)firstPoint.x, (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                savedTex.SetPixels((int)secondPoint.x, (int)secondPoint.y, brushScaleX, brushScaleY, colors);
                savedTex.SetPixels((int)(firstPoint.x + deltaX), (int)firstPoint.y, brushScaleX, brushScaleY, colors);
                savedTex.SetPixels((int)firstPoint.x, (int)(firstPoint.y + deltaY), brushScaleX, brushScaleY, colors);

                savedTex.Apply();
                fieldMaterial.mainTexture = savedTex;
            }
        }

        if (!isBtnDown) {
            firstPoint = new Vector2(-1, -1);
            secondPoint = new Vector2(-1, -1);
            Graphics.CopyTexture(savedTex, fieldTexture);
            fieldMaterial.mainTexture = fieldTexture;
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
        } else if (firstPoint != new Vector2(-1, -1)) {
            Graphics.CopyTexture(fieldTexture, savedTex);
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
                    savedTex.SetPixels((int)(x + firstPoint.x + deltaX / 2), (int)(y + firstPoint.y + deltaY / 2), brushScaleX, brushScaleY, colors);
                    angle += (360f / segments);
                }

                savedTex.Apply();
                fieldMaterial.mainTexture = savedTex;
            }
        }

        if (!isBtnDown) {
            firstPoint = new Vector2(-1, -1);
            secondPoint = new Vector2(-1, -1);
            Graphics.CopyTexture(savedTex, fieldTexture);
            fieldMaterial.mainTexture = fieldTexture;
        }
    }



    IEnumerator SetTextureWhite() {
        yield return new WaitForEndOfFrame();
        fieldTexture = new Texture2D((int)(imgResolution * Field.transform.localScale.z), (int)(imgResolution * Field.transform.localScale.x), TextureFormat.ARGB32, true);
        savedTex = new Texture2D (fieldTexture.width, fieldTexture.height, TextureFormat.ARGB32, true);
        Color[] colors = new Color[fieldTexture.width * fieldTexture.height];
        for (int i = 0; i < fieldTexture.width * fieldTexture.height; i++) {
            colors[i] = Color.white;
        }
        fieldTexture.SetPixels(0, 0, fieldTexture.width, fieldTexture.height, colors);
        fieldTexture.Apply();
        fieldMaterial.mainTexture = fieldTexture;
    }


    public void recalcScales() {
        StartCoroutine(SetTextureWhite());
    }

    
    private void ShowExplorerFields(string[] files)
    {
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
                    byte[] fileData = File.ReadAllBytes(path);
                    Texture2D tex = new Texture2D(0, 0);
                    tex.LoadImage(fileData);
                    float localScaleY = tex.width / imgResolution;
                    float localScaleX = tex.height / imgResolution;
                    Field.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
                    fieldTexture = new Texture2D((int)(imgResolution * Field.transform.localScale.z), (int)(imgResolution * Field.transform.localScale.x), TextureFormat.ARGB32, true);
                    savedTex = new Texture2D((int)(imgResolution * Field.transform.localScale.z), (int)(imgResolution * Field.transform.localScale.x), TextureFormat.ARGB32, true);
                    Graphics.CopyTexture(tex, fieldTexture);
                    Graphics.CopyTexture(fieldTexture, savedTex);
                    fieldMaterial.mainTexture = fieldTexture;
                    Destroy(exp.gameObject);
                }
            );
        }
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


    public void SaveField()
    {
		StartCoroutine(ShowLoadDialogCoroutine());
    }
    
    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");
        
        if (FileBrowser.Success)
        {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(FileBrowser.Result[0]));
            FileBrowserHelpers.CopyFile(FileBrowser.Result[0], destinationPath);
            Debug.Log(destinationPath);
        }
    }


    public void LoadField() {
        string[] files = GetComponent<FileManager>().GetFiles("png");
        ShowExplorerFields(files);
    }


    public void ChangeMode(Dropdown dropdown) {
        mode = dropdown.value;
    }
}
