using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class UIController : MonoBehaviour
{
    // For mouse click imitation
    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    private bool IsCollapsed = false;

    public void BtnClick(GameObject btn)
    {
        switch (btn.name)
        {
            case "BtnCollapse":
                RectTransform rt = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
                rt.position = new Vector2(-rt.position.x, rt.position.y);
                Animator anim = btn.GetComponent(typeof(Animator)) as Animator;
                if (IsCollapsed)
                {
                    anim.SetInteger("UIInterrupt", 1);
                    mouse_event((int)0x00000008, 0, 0, 0, 0); // right down at (0, 0)
                    mouse_event((int)0x00000010, 0, 0, 0, 0); // right up at (0, 0)
                }
                IsCollapsed = !IsCollapsed;
                break;
            default:
                break;
        }
    }
}
