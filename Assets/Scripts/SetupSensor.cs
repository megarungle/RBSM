using UnityEngine;

public class SetupSensor : MonoBehaviour
{
    enum Sensors {
        ColorSensor = 0,
        OtherSensor = 1
    }

    public GameObject ColorSensor;
    public GameObject OtherSensor;
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

    public void SetupOtherSensor() {    
        int idx = (int)Sensors.OtherSensor;
        changeOneSensor(idx);
    }
    
    void FixedUpdate() {
        int indexSensor = getActiveSensor();
        Debug.Log(indexSensor);
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
                                    ColorSensor.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                                } else {
                                    ColorSensor.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                                break;
                            }

                            case (int)Sensors.OtherSensor: {
                                Debug.Log("Other Sensor");
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