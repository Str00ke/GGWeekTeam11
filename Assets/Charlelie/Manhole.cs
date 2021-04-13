using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manhole : MonoBehaviour
{
    public GameObject couple;
    public int nearestNode;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CouplePos()
    {
        return couple.transform.position;
    }
}
