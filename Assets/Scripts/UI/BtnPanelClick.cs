using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPanelClick : MonoBehaviour
{
    [SerializeField] private RobotBuilder builder;

    public void Click()
    {
        builder.SetObjectName(gameObject.name);
    }
}
