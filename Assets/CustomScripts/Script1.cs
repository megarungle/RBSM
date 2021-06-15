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

    private GameObject planeObj;
    private Texture2D plane;

    private ColorSensor cs;

    private Rigidbody rb;

    public float forwardSpeed = 7.0f * 2.7f;
    public float backSpeed = 10.0f * 2.7f;
    public float normalSpeed = 2.0f * 1.7f;

    private Color resColor;
    private int blockX;
    private int blockZ;

    private float maxVelocity = 0.3f;
    private float minVelocity = -0.3f;

    private void MotorMotion(WheelCollider wheel, float speed, int direction)
    {
        wheel.motorTorque = speed * direction;
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        //wheelTransform.position = pos;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(LFWheel, LFWheelTransform);
        UpdateSingleWheel(RFWheel, RFWheelTransform);
        UpdateSingleWheel(RBWheel, RBWheelTransform);
        UpdateSingleWheel(LBWheel, LBWheelTransform);
    }

    private void NormalMotion()
    {
        LBWheel.motorTorque = normalSpeed * 1;
        RBWheel.motorTorque = normalSpeed * 1;
        LFWheel.motorTorque = normalSpeed * 1;
        RFWheel.motorTorque = normalSpeed * 1;
    }

    private void LeftFrontMotion()
    {
        RFWheel.motorTorque = backSpeed * -1;
        RBWheel.motorTorque = backSpeed * -1;
        LFWheel.motorTorque = forwardSpeed * 1;
        LBWheel.motorTorque = forwardSpeed * 1;
    }

    private void RightFrontMotion()
    {
        RFWheel.motorTorque = forwardSpeed * 1;
        RBWheel.motorTorque = forwardSpeed * 1;
        LFWheel.motorTorque = backSpeed * -1;
        LBWheel.motorTorque = backSpeed * -1;
    }

    private void CheckVelocity()
    {
        if (rb.velocity.x > maxVelocity)
        {
            rb.velocity = new Vector3(maxVelocity, rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.y > maxVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxVelocity, rb.velocity.z);
        }
        if (rb.velocity.z > maxVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxVelocity);
        }
        //////////////////////////////////////////////
        if (rb.velocity.x < minVelocity)
        {
            rb.velocity = new Vector3(minVelocity, rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.y < minVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, minVelocity, rb.velocity.z);
        }
        if (rb.velocity.z < minVelocity)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, minVelocity);
        }
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

        rb = gameObject.GetComponent<Rigidbody>();

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

            if (gObj.name.Contains("SensorColor"))
            {
                GameObject tmp = gObj.transform.Find("Sensor").gameObject;
                cs = tmp.GetComponent<ColorSensor>();
            }
        }

        planeObj = GameObject.Find("Plane");
        plane = planeObj.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        plane.filterMode = FilterMode.Point;
        if (plane.width > plane.height)
        {
            float ratio = plane.width / plane.height;
            blockX = (int)(planeObj.transform.localScale.z * 2) * (int)ratio;
            blockZ = (int)(planeObj.transform.localScale.x * 2);
        }
        else
        {
            float ratio = plane.height / plane.width;
            blockX = (int)(planeObj.transform.localScale.z * 2);
            blockZ = (int)(planeObj.transform.localScale.x * 2) * (int)ratio;
        }
    }

    void FixedUpdate()
    {
        //UpdateWheels(0.5f);

        Color[] colors;
        colors = plane.GetPixels((int)(cs.x * plane.width), (int)(cs.z * plane.height), blockX, blockZ);

        float avgR = 0;
        float avgG = 0;
        float avgB = 0;
        for (int i = 0; i < colors.Length; ++i)
        {
            avgR += (float)colors[i][0];
            avgG += (float)colors[i][1];
            avgB += (float)colors[i][2];
        }
        avgR /= (colors.Length);
        avgG /= (colors.Length);
        avgB /= (colors.Length);

        resColor = new Color(avgR, avgB, avgG);

        if ((float)resColor[0] >= 0.7f && (float)resColor[1] >= 0.7f &&
            (float)resColor[2] >= 0.7f)
        {
            NormalMotion();
        }
        else
        {
            LeftFrontMotion();
        }

        CheckVelocity();
        UpdateWheels();
    }
}
