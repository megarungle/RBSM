using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script1 : MonoBehaviour
{
    private WheelCollider LeftWheelBack;
    private WheelCollider RightWheelBack;
    private WheelCollider LeftWheelFront;
    private WheelCollider RightWheelFront;

    public float forwardSpeed = 100.0f;
    public float backSpeed = 30.0f;
    public float normalSpeed = 1.0f;

    private Color resColor;

    private void MotorMotion(WheelCollider wheel, float speed, int direction)
    {
        wheel.motorTorque = speed * direction;
    }

    void Start()
    {
        Component[] cmpnts = gameObject.transform.GetComponentsInChildren(typeof(WheelCollider));
        LeftWheelBack = (WheelCollider)cmpnts[0];
        RightWheelBack = (WheelCollider)cmpnts[1];
        LeftWheelFront = (WheelCollider)cmpnts[2];
        RightWheelFront = (WheelCollider)cmpnts[3];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MotorMotion(LeftWheelBack, forwardSpeed * 19.8f, 1);
            MotorMotion(RightWheelBack, forwardSpeed * 19.8f, 1);
            MotorMotion(LeftWheelFront, forwardSpeed * 19.8f, 1);
            MotorMotion(RightWheelFront, forwardSpeed * 19.8f, 1);
        }
    }
}
