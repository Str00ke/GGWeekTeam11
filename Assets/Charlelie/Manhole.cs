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

    public int CheckNearNode(Vector3 playerPos)
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(playerPos, 5);
        return GetNearestNode(col, playerPos);
    }

    int GetNearestNode(Collider2D[] col, Vector3 pos)
    {
        GameObject nearest = null;
        float minDist = 9999;

        foreach(Collider2D go in col)
        {
            if (go.gameObject != gameObject)
            {
                float tmpDist = Vector3.Distance(go.gameObject.transform.position, pos);
                if (tmpDist < minDist)
                {
                    minDist = tmpDist;
                    nearest = go.gameObject;
                }
            }
            
        }
        return nearest.GetComponent<Node>().index;
    }
}
