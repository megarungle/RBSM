using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public GameObject Field;

    private Material fieldMaterial;
    private Texture2D fieldTexture;

    private float localScaleX = 1f;
    private float localScaleY = 1f;

    private int imgResolution = 256;

    void Start()
    {
        MeshRenderer renderer = Field.GetComponent<MeshRenderer>();
        fieldMaterial = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    // UI handlers
    public void SetField() {
        Texture2D tex = GetComponent<FileManager>().GetTexture();
        localScaleY = tex.width / imgResolution;
        localScaleX = tex.height / imgResolution;
        Field.transform.localScale = new Vector3(localScaleX, 1, localScaleY);
        fieldMaterial.mainTexture = tex;
    }
}
