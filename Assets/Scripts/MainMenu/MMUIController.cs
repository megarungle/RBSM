using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.SceneManagement;

public class MMUIController : MonoBehaviour
{
    public GameObject panelDrawer;
    public GameObject panelEditor;
    public GameObject panelBuilder;
    public GameObject panelSimulator;

    public ScreenFader screenFader;

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

    private IEnumerator ChangeScene(string scene)
    {
        screenFader.fadeState = ScreenFader.FadeState.In;

        yield return new WaitForSeconds(2.0f);
        
        SceneManager.LoadScene(scene);
    }

    public void BtnClick(string bName)
    {
        switch (bName)
        {
            // Left menu
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
            // Menu on right panel
            case "StartDrawing":
                {
                    StartCoroutine(ChangeScene("MatDrawer"));
                    break;
                }
            case "StartPlacement":
                {
                    StartCoroutine(ChangeScene("MatEdit"));
                    break;
                }
            case "StartBuilding":
                {
                    StartCoroutine(ChangeScene("RobotConstructor"));
                    break;
                }
            case "StartProgramming":
                {
                    break;
                }
            case "StartSimulation":
                {
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
