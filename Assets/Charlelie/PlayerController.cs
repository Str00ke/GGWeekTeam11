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
    public int playerPos, playerPosParent;
    Direction direction = new Direction();
    HorDir horDir = new HorDir();
    VerDir verDir = new VerDir();
    bool isAtNode = true;
    Vector3 playerNodeStart, playerNodeEnd;
    float progresion;

    float moveHor, moveVer;
    void Start()
    {
        nodeManager = FindObjectOfType<NodesManager>();

        playerPos = 55;
        transform.position = nodeManager.nodes[playerPos].transform.position;
        
    }

    void Update()
    {
        UpdateDirEnums();
        float value = 1;
        moveHor = Input.GetAxisRaw("Horizontal");
        moveVer = Input.GetAxisRaw("Vertical");

        
        if (isAtNode)
        {
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
                if (((playerPos + 10) + 1) % 10 != 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + 1].transform.position;
                    playerPos += 1;
                }
            }
            else if (moveHor < 0)
            {
                if (!((playerPos / 10) > ((playerPos - 1) / 10)))
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
                if (playerPos + 10 < nodeManager.nodes.Length)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + 10].transform.position;
                    playerPos += 10;
                }
            }
            else if (moveVer > 0)
            {
                if (playerPos - 10 >= 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos - 10].transform.position;
                    playerPos -= 10;
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

    
}
