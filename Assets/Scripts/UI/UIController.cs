using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

public class UIController : MonoBehaviour
{
    /*
    // For mouse click imitation
    [DllImport("user32.dll")]
    private static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);
    [Flags]
    enum MouseFlags
    {
        Move = 0x0001, LeftDown = 0x0002, LeftUp = 0x0004, RightDown = 0x0008,
        RightUp = 0x0010, Absolute = 0x8000
    };
    */

    private bool IsCollapsed = false;
    public GameObject panelFuncElems;
    public GameObject panelBalks;
    public GameObject panelThinBalks;
    public GameObject panelConnectors;
    public GameObject panelWheels;

    private void HidePanels()
    {
        panelFuncElems.SetActive(false);
        panelBalks.SetActive(false);
        panelThinBalks.SetActive(false);
        panelConnectors.SetActive(false);
        panelWheels.SetActive(false);
    }

    private void Start()
    {
        HidePanels();
    }

    IEnumerator ResetActiveAfterAnimation(float animationLen, GameObject btn, Animator btnAnimator, Transform transform, bool IsCollapsed)
    {
        GameObject btnFuncElems = transform.GetChild(1).gameObject;
        GameObject btnBalks = transform.GetChild(2).gameObject;
        GameObject btnThinBalks = transform.GetChild(3).gameObject;
        GameObject btnConnectors = transform.GetChild(4).gameObject;
        GameObject btnWheels = transform.GetChild(5).gameObject;

        // Waiting end of animation
        yield return new WaitForSeconds(animationLen);

        //const int x = 32000;
        //const int y = 32000;

        // Imitate mouse click at the center of the screen
        //mouse_event(MouseFlags.Absolute | MouseFlags.Move, x, y, 0, UIntPtr.Zero);
        //mouse_event(MouseFlags.Absolute | MouseFlags.LeftDown, x, y, 0, UIntPtr.Zero);
        //mouse_event(MouseFlags.Absolute | MouseFlags.RightUp, x, y, 0, UIntPtr.Zero);

        btnFuncElems.SetActive(IsCollapsed);
        btnBalks.SetActive(IsCollapsed);
        btnThinBalks.SetActive(IsCollapsed);
        btnConnectors.SetActive(IsCollapsed);
        btnWheels.SetActive(IsCollapsed);

        if (IsCollapsed)
        {
            // Refresh button
            btn.SetActive(false);
            btn.SetActive(true);
        }
    }

    public void BtnClick(GameObject btn)
    {
        switch (btn.name)
        {
            case "BtnCollapse":
                {
                    Animator btnAnimator = btn.GetComponent(typeof(Animator)) as Animator;

                    // Return if animation did not work
                    if (btnAnimator.GetBool("IsNormal"))
                    {
                        return;
                    }

                    Animation animation = gameObject.GetComponent(typeof(Animation)) as Animation;

                    if (IsCollapsed)
                    {
                        // Interrupt button animation (release pressed trigger for this button)
                        btnAnimator.SetInteger("UIInterrupt", 1);
                        // Panel animation
                        animation.Play("Uncollapse");
                        // Hide UI
                        StartCoroutine(ResetActiveAfterAnimation(animation.GetClip("Uncollapse").length, btn, btnAnimator, transform, IsCollapsed));
                    }
                    else
                    {
                        HidePanels();
                        // Panel animation
                        animation.Play("Collapse");
                        // Unhide UI
                        StartCoroutine(ResetActiveAfterAnimation(animation.GetClip("Collapse").length, btn, btnAnimator, transform, IsCollapsed));
                    }

                    IsCollapsed = !IsCollapsed;
                    break;
                }
            case "BtnFuncElems":
                {
                    HidePanels();
                    panelFuncElems.SetActive(true);
                    break;
                }
            case "BtnBalks":
                {
                    HidePanels();
                    panelBalks.SetActive(true);
                    break;
                }
            case "BtnThinBalks":
                {
                    HidePanels();
                    panelThinBalks.SetActive(true);
                    break;
                }
            case "BtnConnectors":
                {
                    HidePanels();
                    panelConnectors.SetActive(true);
                    break;
                }
            case "BtnWheels":
                {
                    HidePanels();
                    panelWheels.SetActive(true);
                    break;
                }
            default:
                break;
        }
    }
}
