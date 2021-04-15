using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayType
{
    WATCHER,
    RUNNER
}
public class NodesManager : MonoBehaviour
{
    public GameObject node;
    public GameObject[] nodes;

    public GameObject police;
    public GameObject player;

    public int width = 10;
    public int height = 10;

    public int policeNbr = 5;

    public PlayType playType;

    MiniGameManager miniGameManager;
    private void Awake()
    {
        playType = new PlayType();
    }
    
    void Start()
    {
        miniGameManager = FindObjectOfType<MiniGameManager>();
        nodes = new GameObject[width * height];
        Vector3 vec = new Vector3(-width, 8, 0);
        for (int i = 0; i < width * height; ++i)
        {
            if (i % width == 0)
            {
                vec.y -= 2;
                vec.x = -width;
            }
            nodes[i] = Instantiate(node, vec, transform.rotation);
            nodes[i].GetComponent<Node>().SetIndex(i);
            vec.x += 2;

        }
        for (int i = 0; i < policeNbr; ++i)
        {            
            Instantiate(police/*, nodes[(policePos)].transform.position, transform.rotation*/);
        }        
        Instantiate(player/*, nodes[(policePos)].transform.position, transform.rotation*/);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Busted()
    {
        Debug.Log("BUSTEEEEEED");
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        PolicePatrol[] policeCars = FindObjectsOfType<PolicePatrol>();
        foreach (PolicePatrol pP in policeCars)
        {
            Destroy(pP.gameObject);
        }
        Destroy(player);
    }
}
