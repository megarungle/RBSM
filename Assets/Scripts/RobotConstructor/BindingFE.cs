using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingFE : MonoBehaviour
{
    public string slot;

    void Start()
    {
        slot = null;
    }

    public void SetSlot(string s)
    {
        slot = s;
    }
}
