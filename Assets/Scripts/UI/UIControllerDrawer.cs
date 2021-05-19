using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

public class UIControllerDrawer : MonoBehaviour
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
    public GameObject panelEditor;

    private void HidePanels()
    {
        panelEditor.SetActive(true);
    }

    private void Start()
    {
        HidePanels();
    }

    IEnumerator ResetActiveAfterAnimation(float animationLen, GameObject btn, Animator btnAnimator, Transform transform, bool IsCollapsed)
    {
        // Waiting end of animation
        yield return new WaitForSeconds(animationLen);


        if (IsCollapsed)
        {
            // Refresh button
            btn.SetActive(false);
            btn.SetActive(true);
        }
    }

    public void BtnClick(GameObject btn)
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
    }
}
