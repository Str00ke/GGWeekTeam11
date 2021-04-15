using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
    VERTICAL,
    HORIZONTAL
}
enum HorDir
{
    LEFT,
    RIGHT
}
enum VerDir
{
    UP,
    DOWN
}
public class PlayerController : MonoBehaviour
{
    NodesManager nodeManager;
    MiniGameManager miniGameManager;
    public int playerPos, playerPosParent;
    Direction direction = new Direction();
    HorDir horDir = new HorDir();
    VerDir verDir = new VerDir();
    bool isAtNode = true;
    Vector3 playerNodeStart, playerNodeEnd;
    float progresion;
    bool isinHacking = false;
    public bool isInHole = false;
    GameObject nearManHole;
    float moveHor, moveVer;
    Animator anim;
    SpriteRenderer sR;
    void Start()
    {
        nodeManager = FindObjectOfType<NodesManager>();
        miniGameManager = FindObjectOfType<MiniGameManager>();

        anim = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();

        playerPos = 55;
        transform.position = nodeManager.nodes[playerPos].transform.position;
        
    }

    void Update()
    {
        if (isAtNode)
        {
            anim.SetBool("isMoving", false);
        } else
        {
            anim.SetBool("isMoving", true);
            if (direction == Direction.VERTICAL)
            {
                anim.SetFloat("Horizontal", 0);
                if (verDir == VerDir.UP)
                {
                    anim.SetFloat("Vertical", -1);
                } else
                {
                    anim.SetFloat("Vertical", 1);
                }
            } else
            {
                anim.SetFloat("Vertical", 0);
                if (horDir == HorDir.RIGHT)
                {
                    anim.SetFloat("Horizontal", 1);
                }
                else
                {
                    anim.SetFloat("Horizontal", -1);
                }
            }
        }

        if (isinHacking || isInHole)
            return;

        int layerMask = (LayerMask.GetMask("Manhole"));
        Collider2D col = Physics2D.OverlapCircle(transform.position, 2, layerMask);
        if (col && col != nearManHole)
            nearManHole = col.gameObject;
        if (col && Input.GetKeyDown(KeyCode.E))
        {
            isinHacking = true;
            miniGameManager.StartMiniGame();
        }



        UpdateDirEnums();
        float value = 1;
        moveHor = Input.GetAxisRaw("Horizontal");
        moveVer = Input.GetAxisRaw("Vertical");

        
        if (isAtNode)
        {
            playerPosParent = playerPos;
            progresion = 0;
            playerNodeStart = nodeManager.nodes[playerPos].transform.position;
            if (SelectNode()) isAtNode = false;
            // TODO: Fix bug if player at map top and input up player goes horizontal
        } else {
            switch (direction)
            {
                case (Direction.HORIZONTAL):
                    UpdateProgression(value);
                    break;

                case (Direction.VERTICAL):
                    UpdateProgression(value);
                    break;
            }
                
            if (transform.position == playerNodeEnd ) {
                playerNodeStart = transform.position;               
                isAtNode = true;
            }
            else if (transform.position == playerNodeStart)
            {
                playerPos = playerPosParent;
                isAtNode = true;
            }
        }
        
    }

    private void UpdateProgression(float value)
    {
        if (direction == Direction.HORIZONTAL)
        {
            if (playerNodeEnd.x < playerNodeStart.x)
            {
                if (horDir == HorDir.RIGHT) progresion -= (value * 10 * Time.deltaTime) / 10;
                else progresion += (value * 10 * Time.deltaTime) / 10;
            }
            else
            {
                if (horDir == HorDir.LEFT) progresion -= (value * 10 * Time.deltaTime) / 10;
                else progresion += (value * 10 * Time.deltaTime) / 10;
            }
        }
        else if (direction == Direction.VERTICAL)
        {
            if (playerNodeEnd.y < playerNodeStart.y)
            {
                if (verDir == VerDir.UP) progresion -= (value * 10 * Time.deltaTime) / 10;
                else progresion += (value * 10 * Time.deltaTime) / 10;
            }
            else
            {
                if (verDir == VerDir.DOWN) progresion -= (value * 10 * Time.deltaTime) / 10;
                else progresion += (value * 10 * Time.deltaTime) / 10;
            }
        }
        transform.position = MovePlayer(playerNodeStart, playerNodeEnd, progresion);
    }

    Vector3 MovePlayer(Vector3 playerNodeStart, Vector3 playerNodeEnd, float progression)
    {
        return Vector3.Lerp(playerNodeStart, playerNodeEnd, progression);
    }

    bool SelectNode()
    {
        if (moveHor != 0 && moveVer == 0)
        {
            playerPosParent = playerPos;
            if (moveHor > 0)
            {
                if (((playerPos + nodeManager.width) + 1) % nodeManager.width != 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + 1].transform.position;
                    playerPos += 1;
                }
            }
            else if (moveHor < 0)
            {
                if (!((playerPos / nodeManager.width) > ((playerPos - 1) / nodeManager.width)))
                {
                    playerNodeEnd = nodeManager.nodes[playerPos - 1].transform.position;
                    playerPos -= 1;
                }
            }
            direction = Direction.HORIZONTAL;
            return true;
        }
        else if (moveVer != 0 && moveHor == 0)
        {
            playerPosParent = playerPos;
            if (moveVer < 0)
            {
                if (playerPos + nodeManager.width < nodeManager.nodes.Length)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + nodeManager.width].transform.position;
                    playerPos += nodeManager.width;
                }
            }
            else if (moveVer > 0)
            {
                if (playerPos - nodeManager.width >= 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos - nodeManager.width].transform.position;
                    playerPos -= nodeManager.width;
                }
            }
            direction = Direction.VERTICAL;
            return true;
        }
        return false;
    }

    void UpdateDirEnums()
    {
        if (horDir == HorDir.LEFT && moveHor > 0) horDir = HorDir.RIGHT;

        if (horDir == HorDir.RIGHT && moveHor < 0) horDir = HorDir.LEFT;

        if (verDir == VerDir.UP && moveVer < 0) verDir = VerDir.DOWN;

        if (verDir == VerDir.DOWN && moveVer > 0) verDir = VerDir.UP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2);
    }


    public void HackingSuccess()
    {
       StartCoroutine(MovePlayerInManhole(nearManHole.transform.position, nearManHole.GetComponent<Manhole>().CouplePos(), nearManHole.GetComponent<Manhole>().couple.GetComponent<Manhole>().nearestNode));
       isInHole = true;
       isinHacking = false;
    }

    IEnumerator MovePlayerInManhole(Vector3 startPos, Vector3 targetPos, int node)
    {
        float time = 0;
        float duration = 5f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        duration = 0.5f;
        startPos = targetPos;
        targetPos = nodeManager.nodes[node].transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        playerPos = node;
        isInHole = false;
    }

}
