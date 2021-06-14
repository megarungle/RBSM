using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIMBtnsClick : MonoBehaviour
{
    [SerializeField] private SelectPath uiController;

    public void Click()
    {
        uiController.BtnClick(gameObject);
    }
}
