using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    #region Variables
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject settingsMenu;
    #endregion

    public void OnStart()
    {
        levelLoader.StartTransition(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void OnSettings()
    {
        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
