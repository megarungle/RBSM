using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;

public class SelectPath : MonoBehaviour
{
    private Dictionary<string, string> type = new Dictionary<string, string>(3);
    private Dictionary<string, string> filterName = new Dictionary<string, string>(3);
    private Dictionary<string, string> path = new Dictionary<string, string>(3);
    private Dictionary<string, string> desc = new Dictionary<string, string>(3);

    public string pathRobot;
    public string pathScript;
    public string pathEnv;
    public bool start;

    void Start()
    {
        pathRobot = null;
        pathScript = null;
        pathEnv = null;
        start = false;

        type.Add("BtnRobot", ".json");
        type.Add("BtnScript", ".cs");
        type.Add("BtnEnv", ".json");
        filterName.Add("BtnRobot", "Robots");
        filterName.Add("BtnScript", "Scripts");
        filterName.Add("BtnEnv", "Environments");
        path.Add("BtnRobot", Application.dataPath + "/RobotsJson/");
        path.Add("BtnScript", Application.dataPath);
        path.Add("BtnEnv", Application.dataPath + "/MatsJson/");
        desc.Add("BtnRobot", "Select robot");
        desc.Add("BtnScript", "Select script");
        desc.Add("BtnEnv", "Select environment");
    }

    private IEnumerator ShowLoadDialogCoroutine(GameObject btn)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, path[btn.name], null,
            desc[btn.name], "Load");

        if (FileBrowser.Success)
        {
            Debug.Log(FileBrowser.Result[0]);

            switch (btn.name)
            {
                case "BtnRobot":
                    pathRobot = FileBrowser.Result[0];
                    break;
                case "BtnScript":
                    pathScript = FileBrowser.Result[0];
                    break;
                case "BtnEnv":
                    pathEnv = FileBrowser.Result[0];
                    break;
                default:
                    break;
            }
        }
    }

    public void BtnClick(GameObject btn)
    {
        if (btn.name == "BtnStart")
        {
            if (pathRobot != null && pathScript != null && pathEnv != null)
            {
                start = true;
                gameObject.SetActive(false);
            }
        }
        else
        {
            FileBrowser.SetFilters(true, new FileBrowser.Filter(filterName[btn.name], type[btn.name]));
            FileBrowser.SetDefaultFilter(type[btn.name]);
            FileBrowser.AddQuickLink("RBSM", Application.dataPath, null);

            StartCoroutine(ShowLoadDialogCoroutine(btn));
        }
    }
}
