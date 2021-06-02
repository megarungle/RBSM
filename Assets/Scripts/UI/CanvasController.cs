using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject panel;
    public GameObject popup;

    private GameObject btn1;
    private GameObject btn2;
    private GameObject btn3;
    private GameObject btn4;
    private GameObject btnA;
    private GameObject btnB;
    private GameObject btnC;

    private List<GameObject> btns = new List<GameObject>(6);

    void Start()
    {
        btn1 = popup.transform.GetChild(0).gameObject;
        btn2 = popup.transform.GetChild(1).gameObject;
        btn3 = popup.transform.GetChild(2).gameObject;
        btn4 = popup.transform.GetChild(3).gameObject;
        btnA = popup.transform.GetChild(4).gameObject;
        btnB = popup.transform.GetChild(5).gameObject;
        btnC = popup.transform.GetChild(6).gameObject;

        btn1.SetActive(false);
        btn2.SetActive(false);
        btn3.SetActive(false);
        btn4.SetActive(false);
        btnA.SetActive(false);
        btnB.SetActive(false);
        btnC.SetActive(false);

        btns.Add(btn1);
        btns.Add(btn2);
        btns.Add(btn3);
        btns.Add(btn4);
        btns.Add(btnA);
        btns.Add(btnB);
        btns.Add(btnC);

        popup.SetActive(false);
    }

    public void OpenPopUp(List<string> slots)
    {
        popup.SetActive(true);

        foreach (string slot in slots)
        {
            foreach (GameObject btn in btns)
            {
                if (btn.name == slot)
                {
                    btn.SetActive(true);
                }
            }
        }
    }

    public void ClosePopUp()
    {
        btn1.SetActive(false);
        btn2.SetActive(false);
        btn3.SetActive(false);
        btn4.SetActive(false);
        btnA.SetActive(false);
        btnB.SetActive(false);
        btnC.SetActive(false);

        popup.SetActive(false);
    }
}
