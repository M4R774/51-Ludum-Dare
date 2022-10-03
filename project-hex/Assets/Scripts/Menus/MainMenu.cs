using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject quitButton;
    void Start()
    {
        #if UNITY_WEBGL
        quitButton.SetActive(false);
        #endif

    }
    public string gameSceneName;

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
