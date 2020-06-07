using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    public GameObject leftPanel;
    public GameObject rightPanel;

    public void ChangeStatus(bool status)
    {
        leftPanel.SetActive(status);
        rightPanel.SetActive(status);
    }
}
