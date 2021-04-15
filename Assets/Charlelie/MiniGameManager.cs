using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public List<GameObject> miniGameList = new List<GameObject>();
    PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController == null && FindObjectOfType<PlayerController>())
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    public void StartMiniGame()
    {
        int miniGameNbr= Random.Range(0, miniGameList.Count);
        Instantiate(miniGameList[miniGameNbr]);
    }

    public void Success()
    {
        playerController.HackingSuccess();
    }
}
