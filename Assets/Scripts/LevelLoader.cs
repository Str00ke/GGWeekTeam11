using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    #region Variables
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime;
    #endregion
    public void StartTransition(int Index)
    {
        StartCoroutine(SceneTransition(Index));
    }

    IEnumerator SceneTransition(int sceneIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
