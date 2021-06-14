using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.IO;
using TMPro;
using SimpleFileBrowser;

[System.Serializable]
public class ModuleState
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string slot;

    public ModuleState(string n, Vector3 pos, Quaternion rot, string s)
    {
        name = n;
        position = pos;
        rotation = rot;
        slot = s;
    }
}

public class RobotSerializer : MonoBehaviour
{
    public GameObject Robot;
    public GameObject FileName;

    private string saveDir;

    void Start()
    {
        saveDir = Application.dataPath + "/RobotsJson/";
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, saveDir, null,
            "Select robot", "Load");

        if (FileBrowser.Success)
        {
            Debug.Log(FileBrowser.Result[0]);

            string path = "Prefabs models";

            string json = File.ReadAllText(FileBrowser.Result[0]);
            ModuleState[] modulesParams;
            modulesParams = JsonHelper.FromJson<ModuleState>(json);

            foreach (Transform module in Robot.transform)
            {
                Destroy(module.gameObject);
            }

            for (int i = 0; i < modulesParams.Length; i++)
            {
                string name = modulesParams[i].name;
                Vector3 position = modulesParams[i].position;
                Quaternion rotation = modulesParams[i].rotation;
                string slot = modulesParams[i].slot;
                GameObject newModule = Instantiate(Resources.Load(path + parseResource(name)), position, rotation, Robot.transform) as GameObject;
                if (slot != "")
                {
                    StartCoroutine(SetSlot(newModule, slot));
                }
            }
        }
    }

    private IEnumerator SaveToDisk(string path, string data)
    {
        yield return new WaitForSeconds(2);
        File.WriteAllText(path, data);
    }
    
    private IEnumerator SetSlot(GameObject module, string slot)
    {
        yield return new WaitForEndOfFrame();
        module.GetComponent<BindingFE>().slot = slot;
    }
    
    public void SerializeRobot()
    {
        string fileName = (FileName.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI).text + ".json";
        ModuleState[] moduleParams = new ModuleState[Robot.transform.childCount];
        
        for (int i = 0; i < Robot.transform.childCount; i++)
        {
            Transform module = Robot.transform.GetChild(i);
            string name = module.name;
            Vector3 position = module.position;
            Quaternion rotation = module.rotation;
            string slot = module.GetComponent<BindingFE>() ? module.GetComponent<BindingFE>().slot : "";
            ModuleState state = new ModuleState(name, position, rotation, slot);
            moduleParams[i] = state;
        }
        
        string modulesToJson = JsonHelper.ToJson(moduleParams, true);
        
        File.WriteAllText(saveDir + fileName, modulesToJson);
    }
    
    public void DeserializeRobot()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Robots", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.AddQuickLink("RBSM", saveDir, null);

        StartCoroutine(ShowLoadDialogCoroutine());
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
}
