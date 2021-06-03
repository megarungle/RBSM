using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class MMUIController : MonoBehaviour
{
    public GameObject panelDrawer;
    public GameObject panelEditor;
    public GameObject panelBuilder;
    public GameObject panelSimulator;

    private Animation animDrawer;
    private Animation animEditor;
    private Animation animBuilder;
    private Animation animSimulator;

    private static Animation lastAnimation;
    private static GameObject lastPanel;

    private IEnumerator Animate(Animation animation, GameObject panel)
    {
        if (lastAnimation != null)
        {
            lastAnimation.Play("Hide");

            // Waiting end of animation
            yield return new WaitForSeconds(lastAnimation.GetClip("Hide").length);

            lastPanel.SetActive(false);
        }

        panel.SetActive(true);
        animation.Play("Unhide");
        lastAnimation = animation;
        lastPanel = panel;
    }

    public void BtnClick(string bName)
    {
        switch (bName)
        {
            case "BtnDrawer":
                {
                    StartCoroutine(Animate(animDrawer, panelDrawer));
                    break;
                }
            case "BtnEditor":
                {
                    StartCoroutine(Animate(animEditor, panelEditor));
                    break;
                }
            case "BtnBuilder":
                {
                    StartCoroutine(Animate(animBuilder, panelBuilder));
                    break;
                }
            case "BtnSimulator":
                {
                    StartCoroutine(Animate(animSimulator, panelSimulator));
                    break;
                }
            default:
                break;
        }
    }

    void Start()
    {
        animDrawer = panelDrawer.GetComponent(typeof(Animation)) as Animation;
        animEditor = panelEditor.GetComponent(typeof(Animation)) as Animation;
        animBuilder = panelBuilder.GetComponent(typeof(Animation)) as Animation;
        animSimulator = panelSimulator.GetComponent(typeof(Animation)) as Animation;

        panelDrawer.SetActive(false);
        panelEditor.SetActive(false);
        panelBuilder.SetActive(false);
        panelSimulator.SetActive(false);
    }
}
