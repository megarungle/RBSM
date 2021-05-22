using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnRightSideClick : MonoBehaviour
{
    [SerializeField] private UIController controller;
    [SerializeField] private UIControllerMat controllerMat;

    public void Click()
    {
        controller.BtnClick(gameObject);
    }

    public void MatClick()
    {
        controllerMat.BtnClick(gameObject);
    }
}
