using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Behaviour
{
    PATROL,
    CHASE
}
enum CarDir
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public class PolicePatrol : MonoBehaviour
{
    Behaviour behaviour = new Behaviour();
    CarDir carDir = new CarDir();
    NodesManager nodeManager;

    bool isAtNode = true;

    int policePos;
    int policePosParent;
    Vector3 policeDirStart;
    Vector3 policeDirEnd;
    // Start is called before the first frame update
    void Start()
    {
        
        nodeManager = FindObjectOfType<NodesManager>();
        policePos = (nodeManager.height * nodeManager.width) / 2;
        policePosParent = policePos;       
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        transform.position = policeDirStart;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ISATNODE: " + isAtNode);
        if (behaviour == Behaviour.PATROL)
        {
            
            if (isAtNode)
            {
                isAtNode = false;

                List<int> pos = new List<int>();

                if (policePos == 0)
                {
                    if (policePos + 1 != policePosParent) pos.Add(policePos + 1);
                    if (policePos + 10 != policePosParent) pos.Add(policePos + 10);
                }
                else
                {
                    if (policePos - 10 >= 0 && policePos - 10 != policePosParent) pos.Add(policePos - 10);

                    if (policePos + 10 < nodeManager.nodes.Length && policePos + 10 != policePosParent) pos.Add(policePos + 10);

                    if (((policePos + 10) + 1) % 10 != 0 && policePos + 1 != policePosParent) pos.Add(policePos + 1);

                    if (!((policePos / 10) > ((policePos - 1) / 10)) && policePos - 1 != policePosParent) pos.Add(policePos - 1);
                }

                policeDirEnd = ChooseNode(pos);
                StartCoroutine(MovePolicePatrol());
                if (CheckForEngageChase())
                {
                    behaviour = Behaviour.CHASE;
                    PoliceInstigatePosition();
                }
                    
            }

        }
        else if (behaviour == Behaviour.CHASE)
        {   if (isAtNode)
            {
                Debug.Log(isAtNode);
                isAtNode = false;
                if (CheckForEngageChase())
                {
                    PoliceInstigatePosition();                   
                }
                else
                {
                    isAtNode = true;
                    behaviour = Behaviour.PATROL;
                }
                    
                policePosParent = policePos;
            }
            
        }


    }

    Vector3 ChooseNode(List<int> poses)
    {
        Debug.Log("PATROL");
        Vector3 nodeChoosen;
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        int rand = Random.Range(0, poses.Count);
        policePosParent = policePos;
        policePos = poses[rand];
        nodeChoosen = nodeManager.nodes[poses[rand]].transform.position;

        if (policePos == policePosParent + 10) carDir = CarDir.DOWN;
        else if (policePos == policePosParent - 10) carDir = CarDir.UP;
        else if (policePos == policePosParent - 1) carDir = CarDir.LEFT;
        else if (policePos == policePosParent + 1) carDir = CarDir.RIGHT;

        return nodeChoosen;
    }

    IEnumerator MovePolicePatrol()
    {
        Debug.Log("MOVE");
        float time = 0;
        float duration = 0.2f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(policeDirStart, policeDirEnd, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        isAtNode = true;
    }

   bool CheckForEngageChase()
   {
        bool isSeeingPlayer = false;
        int playerPos = FindObjectOfType<PlayerController>().playerPos;
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (behaviour == Behaviour.PATROL)
        {
            if (dist <= 1)
            {
                if (carDir == CarDir.UP && playerPos == policePosParent - 10)
                {
                    isSeeingPlayer = true;
                    policePos -= 10;
                }
                    
                else if (carDir == CarDir.DOWN && playerPos == policePosParent + 10)
                {
                    isSeeingPlayer = true;
                    policePos += 10;
                }
                else if (carDir == CarDir.LEFT && playerPos == policePosParent - 1)
                {
                    isSeeingPlayer = true;
                    policePos -= 1;
                }
                else if (carDir == CarDir.RIGHT && playerPos == policePosParent + 1)
                {
                    isSeeingPlayer = true;
                    policePos += 1;
                }
                else if (policePosParent == playerPos || policePos == playerPos)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
            }
        } else if (behaviour == Behaviour.CHASE)
        {
            if (dist < 0.5f)
            {
                nodeManager.Busted();
            }
            if (dist <= 2)
            {
                if (playerPos == policePos - 10)
                {
                    isSeeingPlayer = true;
                    policePos -= 10;
                }

                else if (playerPos == policePos + 10)
                {
                    isSeeingPlayer = true;
                    policePos += 10;
                }
                else if (playerPos == policePos - 1)
                {
                    isSeeingPlayer = true;
                    policePos -= 1;
                }
                else if (playerPos == policePos + 1)
                {
                    isSeeingPlayer = true;
                    policePos += 1;
                }
                else if (policePos == playerPos)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
            }
            else
                isSeeingPlayer = false;
            Debug.Log(isSeeingPlayer);
        }

        
        return isSeeingPlayer;
   }

    void PoliceInstigatePosition()
    {
        int playerPos = FindObjectOfType<PlayerController>().playerPos;
        policeDirStart = nodeManager.nodes[policePosParent].transform.position;
        policeDirEnd = nodeManager.nodes[playerPos].transform.position;
        StartCoroutine(MovePolicePatrol());
        
    }

    void PoliceChase()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime);
    }
}
