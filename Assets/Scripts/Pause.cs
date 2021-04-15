using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject pauseMenu;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject settingsMenu;
    [HideInInspector]
    public static bool pauseIsActive = false;
    [HideInInspector]
    public bool hasLost = false;
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasLost)
        {
            if (pauseIsActive)
            {
                setPause(false);
            }
            else
            {
                setPause(true);
            }
        }
    }

    private void setPause(bool newPauseValue)
    {
        pauseMenu.SetActive(newPauseValue);
        pauseIsActive = newPauseValue;
        if (newPauseValue)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void OnRestart()
    {
        Time.timeScale = 1f;
        levelLoader.StartTransition(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenu()
    {
        Time.timeScale = 1f;
        levelLoader.StartTransition(0);
    }

    public void OnSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void OnResume()
    {
        setPause(false);
    }
}
