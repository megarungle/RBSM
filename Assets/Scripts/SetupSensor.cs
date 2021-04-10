using UnityEngine;
using UnityEngine.UI;


public class SetupSensor : MonoBehaviour
{
    enum Sensors {
        ColorSensor = 0,
        DistSensor = 1
    }

    public GameObject ColorSensor;
    public GameObject DistSensor;
    public GameObject HUD;
    public GameObject Robot;

    bool[] sensorButtons;

    void Start() {
        sensorButtons = new bool[System.Enum.GetNames(typeof(Sensors)).Length]; // Bool array with size equal to the number of sensors 
    }

    void changeOneSensor(int idx) {
        for (int i = 0; i < sensorButtons.Length; i++) {
            if (i != idx)
                sensorButtons[i] = false;
            else
                sensorButtons[i] = !sensorButtons[idx];
        }
    }

    int getActiveSensor() {
        return System.Array.IndexOf(sensorButtons, true);
    }

    void deactivateAll() {
        for (int i = 0; i < sensorButtons.Length; i++) {
            sensorButtons[i] = false;
        }
    }

    public void SetupColorSensor() {
        int idx = (int)Sensors.ColorSensor;
        changeOneSensor(idx);
    }

    public void SetupDistSensor() {    
        int idx = (int)Sensors.DistSensor;
        changeOneSensor(idx);
    }
    
    void FixedUpdate() {
        int indexSensor = getActiveSensor();
        if (indexSensor != -1) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, 100)) {
                    string ObjectName = hit.collider.name;

                    if (ObjectName == "Body") {
                        switch (indexSensor) {
                            case (int)Sensors.ColorSensor: {
                                Debug.Log("Color Sensor");
                                float sensorWidth = ColorSensor.transform.localScale.y;
                                ColorSensor.transform.parent = Robot.transform;
                                ColorSensor.transform.position = hit.point + hit.normal * sensorWidth / 2;
                                if (hit.normal.y != 0) {
                                    ColorSensor.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                                } else {
                                    ColorSensor.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                                break;
                            }

                            case (int)Sensors.DistSensor: {
                                Debug.Log("Dist Sensor");
                                float sensorWidth = DistSensor.transform.localScale.y;
                                DistSensor.transform.parent = Robot.transform;
                                DistSensor.transform.position = hit.point + hit.normal * sensorWidth / 2;
                                if (hit.normal.y != 0) {
                                    DistSensor.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                                } else {
                                    DistSensor.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                                float dist = DistSensor.GetComponentInChildren<DistSensor>().dist;
                                Debug.Log(dist);
                                break;
                            }

                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}