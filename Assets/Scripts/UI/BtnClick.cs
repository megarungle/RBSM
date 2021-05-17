using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnClick : MonoBehaviour
{
    [SerializeField] private UIController controller;

    public void Click()
    {
        controller.BtnClick(gameObject);
    }
}
