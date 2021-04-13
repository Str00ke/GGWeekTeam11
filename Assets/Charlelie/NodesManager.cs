using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesManager : MonoBehaviour
{
    public GameObject node;
    public GameObject[] nodes;

    public GameObject police;
    public GameObject player;

    public int width = 10;
    public int height = 10;
    // Start is called before the first frame update
    void Start()
    {
        nodes = new GameObject[width * height];
        Vector3 vec = new Vector3(-width, 6, 0);
        for (int i = 0; i < width * height; ++i)
        {
            if (i % 10 == 0)
            {
                vec.y -= 1;
                vec.x = -width;
            }
            nodes[i] = Instantiate(node, vec, transform.rotation);
            vec.x += 1;

        }
        Instantiate(police/*, nodes[(policePos)].transform.position, transform.rotation*/);
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
