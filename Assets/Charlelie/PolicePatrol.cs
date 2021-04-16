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

    bool isCoroutineRunning = false;

    PlayerController playerController;

    int policePos, tmpPos;
    int policePosParent;
    Vector3 policeDirStart;
    Vector3 policeDirEnd;
    Vector3 target;
    public float speed;
    
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        sR = GetComponent<SpriteRenderer>();
        nodeManager = FindObjectOfType<NodesManager>();
        radio = FindObjectOfType<Radio>();
        int randNode = RandStartPos();
        policePos = randNode;
        policePosParent = policePos;       
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        transform.position = policeDirStart;
    }

    int RandStartPos()
    {
        int randNode = Random.Range(0, nodeManager.nodes.Length);
        if (randNode == 82)
            return RandStartPos();
        else return randNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isAtNode)
        {
            target = nodeManager.nodes[playerController.playerPos].transform.position;
        } else
        {
            /*if (isCoroutineRunning)
            {
                StopCoroutine(MovePolicePatrol(policeDirStart, policeDirEnd));
            }*/
            target = playerController.transform.position;
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
                    policeDirStart = transform.position;
                    policeDirEnd = nodeManager.nodes[policePos].transform.position;
                    behaviour = Behaviour.CHASE;
                    speed = 1.5f;
                    if (isCoroutineRunning)
                    {
                        StopCoroutine(MovePolicePatrol());
                        //Debug.Log("Stopped Coroutine");
                    }
                    //Debug.Log("Entering Chase Mode");
                    //Debug.Break();
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
                    speed = 2;
                }
                    
                policePosParent = policePos;
            }
            
        }


    }

    void UpdateCarSprite()
    {
        if (carDir == CarDir.LEFT || carDir == CarDir.RIGHT)
        {
            sR.sprite = carSide;
            if (carDir == CarDir.LEFT)
                sR.flipX = false;
            else
                sR.flipX = true;
        }
        else
        {
            sR.flipX = false;
            if (carDir == CarDir.UP)
            {
                sR.sprite = carUp;
            }
            else
            {
                sR.sprite = carDown;
            }
        }
    }

    Vector3 ChooseNode(List<int> poses)
    {
        //Debug.Log("ChooseNode...");
        Vector3 nodeChoosen;
        policeDirStart = nodeManager.nodes[policePos].transform.position;
        int rand = Random.Range(0, poses.Count);
        policePosParent = policePos;
        tmpPos = poses[rand];
        nodeChoosen = nodeManager.nodes[poses[rand]].transform.position;

        if (tmpPos == policePos + nodeManager.width) carDir = CarDir.DOWN;
        else if (tmpPos == policePos - nodeManager.width) carDir = CarDir.UP;
        else if (tmpPos == policePos - 1) carDir = CarDir.LEFT;
        else if (tmpPos == policePos + 1) carDir = CarDir.RIGHT;

        UpdateCarSprite();

        return nodeChoosen;
    }

    IEnumerator MovePolicePatrol()
    {
        isCoroutineRunning = true;
        float time = 0;
        float duration = speed;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(policeDirStart, policeDirEnd, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        isAtNode = true;
        isCoroutineRunning = false;
        if (behaviour == Behaviour.CHASE)
            policePosParent = policePos;
        else
        {
            policePos = tmpPos;
        }
    }

   bool CheckForEngageChase()
   {
        if (playerController.isInHole) return false;

        bool isSeeingPlayer = false;
        int playerPos = playerController.playerPos;
        int playerPosParent = playerController.playerPosParent;
        int futurePlayerPos = playerController.futurePlayerPos;
        GameObject player = playerController.gameObject;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        
        if (behaviour == Behaviour.PATROL)
        {
            if (/*dist <= 1*/playerController.isAtNode)
            {
                if (carDir == CarDir.UP && (playerPos == policePos - nodeManager.width))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                    
                else if (carDir == CarDir.DOWN && (playerPos == policePos + nodeManager.width))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (carDir == CarDir.LEFT && (playerPos == policePos - 1))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (carDir == CarDir.RIGHT && (playerPos == policePos + 1))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (policePos == playerPos)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
            } /*else
            {
                if (carDir == CarDir.UP && (futurePlayerPos == policePosParent - nodeManager.width || futurePlayerPos == policePos))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }

                else if (carDir == CarDir.DOWN && (futurePlayerPos == policePosParent + nodeManager.width || futurePlayerPos == policePos))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (carDir == CarDir.LEFT && (futurePlayerPos == policePosParent - 1 || futurePlayerPos == policePos))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (carDir == CarDir.RIGHT && (futurePlayerPos == policePosParent + 1 || futurePlayerPos == policePos))
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
            }*/
            
            return isSeeingPlayer;
        } else if (behaviour == Behaviour.CHASE)
        {
            if (dist < 0.5f)
            {
                //nodeManager.Busted();
            }
            if (/*dist <= 2*/ playerController.isAtNode)
            {
                /*if (playerPos == policePos - nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }

                else if (playerPos == policePos + nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (playerPos == policePos - 1)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (playerPos == policePos + 1)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (policePos == playerPos)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
            } else
            {
                if (futurePlayerPos == policePos - nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }

                else if (futurePlayerPos == policePos + nodeManager.width)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (futurePlayerPos == policePos - 1)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }
                else if (futurePlayerPos == policePos + 1)
                {
                    isSeeingPlayer = true;
                    policePos = playerPos;
                }*/
                if (policePos == playerPos)
                {
                    return true;
                }


                if (carDir == CarDir.UP && playerPos == policePos - nodeManager.width) isSeeingPlayer = true;
                else if (carDir == CarDir.DOWN && playerPos == policePos + nodeManager.width) isSeeingPlayer = true;
                else if (carDir == CarDir.LEFT && playerPos == policePos - 1) isSeeingPlayer = true;
                else if (carDir == CarDir.RIGHT && playerPos == policePos + 1) isSeeingPlayer = true;

                if (isSeeingPlayer) policePos = playerPos;
                
            } /*else
            {
                if (carDir == CarDir.UP && futurePlayerPos == policePos - nodeManager.width) isSeeingPlayer = true;
                else if (carDir == CarDir.DOWN && futurePlayerPos == policePos + nodeManager.width) isSeeingPlayer = true;
                else if (carDir == CarDir.LEFT && futurePlayerPos == policePos - 1) isSeeingPlayer = true;
                else if (carDir == CarDir.RIGHT && futurePlayerPos == policePos + 1) isSeeingPlayer = true;

                if (isSeeingPlayer) policePos = futurePlayerPos;
            }*/
            /*else
                isSeeingPlayer = false;*/
            //Debug.Log(isSeeingPlayer);
        }


        return isSeeingPlayer;
    }

    void PoliceInstigatePosition()
    {
        //Debug.Log("Instigate...");
        int playerPos = FindObjectOfType<PlayerController>().playerPos;
        policeDirStart = nodeManager.nodes[policePosParent].transform.position;
        policeDirEnd = nodeManager.nodes[policePos].transform.position;
        /*if (!isCoroutineRunning)
        {
            if (playerController.isAtNode)
                StartCoroutine(MovePolicePatrol(policeDirStart, policeDirEnd));
            else
                StartCoroutine(MovePolicePatrol(policeDirStart, target));
        }*/
        StartCoroutine(MovePolicePatrol());


    }

    void PoliceChase()
    {
        Vector3 playerPos = FindObjectOfType<PlayerController>().transform.position;
        Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime);

        if (playerPos.x > transform.position.x)
        {
            Vector3.MoveTowards(transform.position, Vector2.right, 5 * Time.deltaTime);
            return;
        } else if (playerPos.x < transform.position.x)
        {
            Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime);
            return;
        }

        if (playerPos.y > transform.position.y)
        {
            Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime);
            return;
        } else if (playerPos.y < transform.position.y)
        {
            Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime);
            return;
        }
    }
}
