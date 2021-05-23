using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnPanelClick : MonoBehaviour
{
    [SerializeField] private RobotBuilder builder;

    public void Click()
    {
        string parentName = transform.parent.name; // Get the parent name to establish the category
        parentName = parentName.Replace("Panel", string.Empty);

        builder.SetNames(gameObject.name, parentName);
    }
}
