using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObstacle : MonoBehaviour
{
    public Camera cam;
    public GameObject paramsPanel;

    void Start()
    {
        paramsPanel.SetActive(false);
    }

    public void SelectCube()
    {
        cam.GetComponent<CreateObstacle>().Create(0);
        paramsPanel.SetActive(true);
    }

    public void SelectCylinder()
    {
        cam.GetComponent<CreateObstacle>().Create(1);
        paramsPanel.SetActive(true);
    }

    public void SelectSphere()
    {
        cam.GetComponent<CreateObstacle>().Create(2);
        paramsPanel.SetActive(true);
    }
}
