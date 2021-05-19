using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnRightSideClick : MonoBehaviour
{
    [SerializeField] private UIController controller;
    [SerializeField] private UIControllerDrawer controllerDrawer;

    public void Click()
    {
        controller.BtnClick(gameObject);
    }

    public void DrawerClick()
    {
        controllerDrawer.BtnClick(gameObject);
    }
}
