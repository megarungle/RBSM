using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.SceneManagement;

public class BackToMainMenu : MonoBehaviour
{
    public ScreenFader screenFader;

    private IEnumerator ChangeScene()
    {
        screenFader.fadeState = ScreenFader.FadeState.In;

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ChangeScene());
        }
    }
}
