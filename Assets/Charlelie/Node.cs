using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int index;
    GameObject onNode;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void SetIndex(int value)
    {
        index = value;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetOnNode(GameObject value)
    {
        onNode = value;
    }

    public GameObject GetOnNode()
    {
        return onNode;
    }
}
