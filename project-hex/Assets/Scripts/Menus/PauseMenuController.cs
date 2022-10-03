using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public string nameOfMainMenuScene;
    [SerializeField] GameObject quitButton;

    public void Start()
    {
        Continue();
        #if UNITY_WEBGL
        quitButton.SetActive(false);
        #endif
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuCanvas.activeInHierarchy)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenuCanvas.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        pauseMenuCanvas.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(nameOfMainMenuScene);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
