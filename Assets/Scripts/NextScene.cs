using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public void CreateSimulation()
    {
        Debug.Log("Load MatEditor scene");
        SceneManager.LoadScene("MatEditor");
    }

    public void Samples()
    {
        Debug.Log("Load SelectSample scene");
        SceneManager.LoadScene("SelectSample");
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void BackToMenu()
    {
        Debug.Log("Load MainMenu scene");
        SceneManager.LoadScene("MainMenu");
    }

    public void RingMovement()
    {
        Debug.Log("Load SampleRing scene");
        SceneManager.LoadScene("SampleRing");
    }
}
