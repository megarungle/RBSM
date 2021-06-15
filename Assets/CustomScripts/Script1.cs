using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script1 : MonoBehaviour
{
    private WheelCollider LBWheel;
    private WheelCollider RBWheel;
    private WheelCollider LFWheel;
    private WheelCollider RFWheel;

    private Transform LBWheelTransform;
    private Transform RBWheelTransform;
    private Transform LFWheelTransform;
    private Transform RFWheelTransform;

    public float forwardSpeed = 100.0f;
    public float backSpeed = 30.0f;
    public float normalSpeed = 10.0f;

    private Color resColor;

    private void MotorMotion(WheelCollider wheel, float speed, int direction)
    {
        wheel.motorTorque = speed * direction;
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform, float rotation)
    {
        wheelTransform.Rotate(rotation, 0.0f, 0.0f, Space.World);
    }

    private void UpdateWheels(float rotation)
    {
        UpdateSingleWheel(LFWheel, LFWheelTransform, rotation);
        UpdateSingleWheel(RFWheel, RFWheelTransform, rotation);
        UpdateSingleWheel(RBWheel, RBWheelTransform, rotation);
        UpdateSingleWheel(LBWheel, LBWheelTransform, rotation);
    }

    void Start()
    {
        /*
         __ _______ __
        |W3| FRONT |W4|
        |__|       |__|
         __|       |__
        |W1|NXT    |W2|
        |__|_______|__|

        */

        Component[] cmpnts = gameObject.transform.GetComponentsInChildren(typeof(WheelCollider));
        LBWheel = (WheelCollider)cmpnts[0];
        RBWheel = (WheelCollider)cmpnts[1];
        LFWheel = (WheelCollider)cmpnts[2];
        RFWheel = (WheelCollider)cmpnts[3];

        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            GameObject gObj = gameObject.transform.GetChild(i).gameObject;

            if (gObj.name.Contains("cross") || gObj.name.Contains("hole"))
            {
                switch (gObj.name[gObj.name.Length - 1])
                {
                    case '1':
                        LBWheelTransform = gObj.transform;
                        break;
                    case '2':
                        RBWheelTransform = gObj.transform;
                        break;
                    case '3':
                        LFWheelTransform = gObj.transform;
                        break;
                    case '4':
                        RFWheelTransform = gObj.transform;
                        break;
                    default:
                        break;
                }
            }

            if (gObj.name.Contains("WCollider"))
            {
                switch (gObj.name[gObj.name.Length - 1])
                {
                    case '1':
                        LBWheel = gObj.GetComponent(typeof(WheelCollider)) as WheelCollider;
                        break;
                    case '2':
                        RBWheel = gObj.GetComponent(typeof(WheelCollider)) as WheelCollider;
                        break;
                    case '3':
                        LFWheel = gObj.GetComponent(typeof(WheelCollider)) as WheelCollider;
                        break;
                    case '4':
                        RFWheel = gObj.GetComponent(typeof(WheelCollider)) as WheelCollider;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MotorMotion(LBWheel, normalSpeed, 1);
            MotorMotion(RBWheel, normalSpeed, 1);
            MotorMotion(LFWheel, normalSpeed, 1);
            MotorMotion(RFWheel, normalSpeed, 1);
        }
        UpdateWheels(0.5f);
    }
}
