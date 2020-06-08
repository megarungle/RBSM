using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    public void ChangeStatus(bool status)
    {
        this.gameObject.SetActive(status);
    }
}
