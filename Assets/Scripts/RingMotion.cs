using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingMotion : MonoBehaviour
{
    public WheelCollider RightFrontWheel;
    public WheelCollider RightBackWheel;
    public WheelCollider LeftFrontWheel;
    public WheelCollider LeftBackWheel;

    public float forwardSpeed = 100.0f;
    public float backSpeed = 30.0f;

    public float normalSpeed = 1.0f;

    public Text textColor;
    public Text textMotion;

    Color resColor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        resColor = transform.GetChild(1).GetComponent<RaySensor>().resColor; // Get resColor from Sensor

        textColor.text = "Color: R = " + resColor[0] + ", G = " +
                         resColor[1] + ", B = " + resColor[2];

        if ((float)resColor[0] >= 0.7f && (float)resColor[1] >= 0.7f &&
            (float)resColor[2] >= 0.7f)
        {
            NormalMotion();
            Debug.Log("Normal motion");
            textMotion.text = "Motion: normal";
        }
        else
        {
            LeftFrontMotion();
            Debug.Log("Left front motion");
            textMotion.text = "Motion: left front";
        }
    }

    private void LeftFrontMotion()
    {
        RightFrontWheel.motorTorque = backSpeed * -1;
        RightBackWheel.motorTorque = backSpeed * -1;
        LeftFrontWheel.motorTorque = forwardSpeed * 1;
        LeftBackWheel.motorTorque = forwardSpeed * 1;
    }

    private void RightFrontMotion()
    {
        RightFrontWheel.motorTorque = forwardSpeed * 1;
        RightBackWheel.motorTorque = forwardSpeed * 1;
        LeftFrontWheel.motorTorque = backSpeed * -1;
        LeftBackWheel.motorTorque = backSpeed * -1;
    }

    private void NormalMotion()
    {
        RightFrontWheel.motorTorque = normalSpeed * 1;
        RightBackWheel.motorTorque = normalSpeed * 1;
        LeftFrontWheel.motorTorque = normalSpeed * 1;
        LeftBackWheel.motorTorque = normalSpeed * 1;
    }
}
