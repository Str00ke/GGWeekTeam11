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
    Radio radio;
    bool isAtNode = true;
    SpriteRenderer sR;
    public Sprite carDown, carUp, carSide;

    int policePos;
    int policePosParent;
    Vector3 policeDirStart;
    Vector3 policeDirEnd;
    
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        nodeManager = FindObjectOfType<NodesManager>();
        radio = FindObjectOfType<Radio>();
        int randNode = Random.Range(0, nodeManager.nodes.Length);
        policePos = randNode;
        policePosParent = policePos;       
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        transform.position = policeDirStart;
    }

    // Update is called once per frame
    void Update()
    {
        if (carDir == CarDir.LEFT || carDir == CarDir.RIGHT)
        {
            sR.sprite = carSide;
            if (carDir == CarDir.LEFT)
                sR.flipX = false;
            else
                sR.flipX = true;
        } else
        {
            sR.flipX = false;
            if (carDir == CarDir.UP)
            {
                sR.sprite = carUp;
            } else
            {
                sR.sprite = carDown;
            } 
        }

        if (radio.isOnFrequency)
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, 255);
        else
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, 0);

        if (behaviour == Behaviour.PATROL)
        {
            
            if (isAtNode)
            {
                isAtNode = false;

                List<int> pos = new List<int>();

                if (policePos == 0)
                {
                    if (policePos + 1 != policePosParent) pos.Add(policePos + 1);
                    if (policePos + nodeManager.width != policePosParent) pos.Add(policePos + nodeManager.width);
                }
                else
                {
                    if (policePos - nodeManager.width >= 0 && policePos - nodeManager.width != policePosParent) pos.Add(policePos - nodeManager.width);

                    if (policePos + nodeManager.width < nodeManager.nodes.Length && policePos + nodeManager.width != policePosParent) pos.Add(policePos + nodeManager.width);

                    if (((policePos + nodeManager.width) + 1) % nodeManager.width != 0 && policePos + 1 != policePosParent) pos.Add(policePos + 1);

                    if (!((policePos / nodeManager.width) > ((policePos - 1) / nodeManager.width)) && policePos - 1 != policePosParent) pos.Add(policePos - 1);
                }

                policeDirEnd = ChooseNode(pos);
                StartCoroutine(MovePolicePatrol());
                if (CheckForEngageChase())
                {
                    behaviour = Behaviour.CHASE;
                    //Debug.Log("Entering Chase Mode");
                    //Debug.Break();
                    PoliceInstigatePosition();
                }
                    
            }

        }
        else if (behaviour == Behaviour.CHASE)
        {   if (isAtNode)
            {
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
        Vector3 nodeChoosen;
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        int rand = Random.Range(0, poses.Count);
        policePosParent = policePos;
        policePos = poses[rand];
        nodeChoosen = nodeManager.nodes[poses[rand]].transform.position;

        if (policePos == policePosParent + nodeManager.width) carDir = CarDir.DOWN;
        else if (policePos == policePosParent - nodeManager.width) carDir = CarDir.UP;
        else if (policePos == policePosParent - 1) carDir = CarDir.LEFT;
        else if (policePos == policePosParent + 1) carDir = CarDir.RIGHT;

        return nodeChoosen;
    }

    IEnumerator MovePolicePatrol()
    {
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
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (playerController.isInHole) return false;


        bool isSeeingPlayer = false;
        int playerPos = playerController.playerPos;
        GameObject player = playerController.gameObject;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(/*carDir + "   " + */policePos + "   " + playerPos);
        if (behaviour == Behaviour.PATROL)
        {
            if (/*dist <= 1*/true)
            {
                if (carDir == CarDir.UP && playerPos == policePosParent - nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos -= nodeManager.width;
                }
                    
                else if (carDir == CarDir.DOWN && playerPos == policePosParent + nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos += nodeManager.width;
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
                //nodeManager.Busted();
            }
            if (/*dist <= 2*/ true)
            {
                if (playerPos == policePos - nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos -= nodeManager.width;
                }

                else if (playerPos == policePos + nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos += nodeManager.width;
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
            /*else
                isSeeingPlayer = false;*/
            //Debug.Log(isSeeingPlayer);
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
