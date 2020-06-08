using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObstacle : MonoBehaviour
{
    public Camera cam;

    public void SelectCube()
    {
        cam.GetComponent<CreateObstacle>().Create(0);
    }

    public void SelectCylinder()
    {
        cam.GetComponent<CreateObstacle>().Create(1);
    }

    public void SelectSphere()
    {
        cam.GetComponent<CreateObstacle>().Create(2);
    }
}
