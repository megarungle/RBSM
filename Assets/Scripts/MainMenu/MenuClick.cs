using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClick : MonoBehaviour
{
    [SerializeField] private MMUIController controller;

    public void Click()
    {
        controller.BtnClick(gameObject.name);
    }
}
