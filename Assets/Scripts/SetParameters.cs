using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetParameters : MonoBehaviour
{
    public Camera cam;
    public GameObject sliderSize;
    public GameObject sliderRotation;
    public GameObject dropwdownColor;

    public void UpdateSize()
    {
        float size = sliderSize.GetComponent<Slider>().value;
        cam.GetComponent<CreateObstacle>().UpdateSize(size);
    }

    public void UpdateRotation()
    {
        float rotation = sliderRotation.GetComponent<Slider>().value;
        cam.GetComponent<CreateObstacle>().UpdateRotation(rotation);
    }

    public void UpdateColor()
    {
        int color = dropwdownColor.GetComponent<Dropdown>().value;
        cam.GetComponent<CreateObstacle>().UpdateColor(color);
    }
}
