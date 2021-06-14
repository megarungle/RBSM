using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LoadRobot : MonoBehaviour
{

    void Start()
    {
        DeserializeRobot("C:\\Users\\Anton\\Desktop\\RBSM\\Assets\\RobotsJson", "Test.json");
    }
    
    private string parseResource(string moduleName)
    {
        moduleName = moduleName.Substring(0, moduleName.Length - 7); // discarding "(Clone)"
        if (moduleName.Contains("Liftarm"))
        {
            return "/Balks/" + moduleName;
        } else if (moduleName.Contains("Pin") || moduleName == "Axle3Stub")
        {
            return "/Connectors/" + moduleName;
        } else if (moduleName.Contains("NXT") || moduleName.Contains("Motor") || moduleName.Contains("Sensor"))
        {
            return "/FuncElems/" + moduleName;
        } else if (moduleName.Contains("Axle"))
        {
            return "/Axles/" + moduleName;
        } else if (moduleName.Contains("cross") || moduleName.Contains("hole"))
        {
            return "/Wheels/" + moduleName;
        } else return "";
    }
    
    private IEnumerator SetSlot(GameObject module, string slot)
    {
        yield return new WaitForEndOfFrame();
        module.GetComponent<BindingFE>().slot = slot;
    }

    private void DeserializeRobot(string path, string fileName)
    {
        string json = File.ReadAllText(path + '\\' + fileName);
        ModuleState[] modulesParams;
        modulesParams = JsonHelper.FromJson<ModuleState>(json);

        for (int i = 0; i < modulesParams.Length; i++)
        {
            string name = modulesParams[i].name;
            Vector3 position = modulesParams[i].position;
            Quaternion rotation = modulesParams[i].rotation;
            string slot = modulesParams[i].slot;
            GameObject newModule = Instantiate(Resources.Load(path + parseResource(name)), position, rotation, gameObject.transform) as GameObject;
            if (slot != "")
            {
                StartCoroutine(SetSlot(newModule, slot));
            }
        }
    }
}
