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
    AudioManager audioManager;
    AudioSource audioSource;
    GameObject cam;
    public int playerPos, playerPosParent, posToUpdate, futurePlayerPos;
    Direction direction = new Direction();
    HorDir horDir = new HorDir();
    VerDir verDir = new VerDir();
    public bool isAtNode = true;
    bool isFinish = false;
    Vector3 playerNodeStart, playerNodeEnd;
    float progresion;
    bool isinHacking = false;
    public bool isInHole = false;
    GameObject nearManHole;
    GameObject finishLine, finishFar;
    float moveHor, moveVer;
    Animator anim;
    SpriteRenderer sR;
    public AudioClip walk;
    void Start()
    {
        nodeManager = FindObjectOfType<NodesManager>();
        miniGameManager = FindObjectOfType<MiniGameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        anim = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walk;
        audioSource.Play();
        cam = transform.GetChild(0).gameObject;
        finishFar = GameObject.Find("FinishLine").gameObject.transform.GetChild(0).gameObject;

        playerPos = 5;
        playerPosParent = playerPos;
        transform.position = nodeManager.nodes[playerPos].transform.position;        
    }

    void Update()
    {
        if (isFinish) return;

        Debug.Log(audioSource.isPlaying);
        if (isAtNode)
        {
            audioSource.Pause();
            anim.SetBool("isMoving", false);
        } else
        {
            anim.SetBool("isMoving", true);
            audioSource.Play();
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
        if (col && col.gameObject.name == "FinishLine")
           StartCoroutine(WinGame());
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
            //playerPosParent = playerPos;
            progresion = 0;
            playerNodeStart = nodeManager.nodes[playerPos].transform.position;
            if (SelectNode()) {
                isAtNode = false;
                playerPosParent = playerPos;
            } 
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
                //playerPosParent = playerPos;
                isAtNode = true;
                UpdatePlayerPos();
            }
            else if (transform.position == playerNodeStart)
            {
                playerPos = playerPosParent;
                isAtNode = true;
                //UpdatePlayerPos();
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
        posToUpdate = 0;
        if (moveHor != 0 && moveVer == 0)
        {
            if (moveHor > 0)
            {
                if (((playerPos + nodeManager.width) + 1) % nodeManager.width != 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + 1].transform.position;
                    posToUpdate += 1;
                }
            }
            else if (moveHor < 0)
            {
                if (!((playerPos / nodeManager.width) > ((playerPos - 1) / nodeManager.width)))
                {
                    playerNodeEnd = nodeManager.nodes[playerPos - 1].transform.position;
                    posToUpdate -= 1;
                }
            }
            direction = Direction.HORIZONTAL;
            futurePlayerPos = playerPos + posToUpdate;
            return true;
        }
        else if (moveVer != 0 && moveHor == 0)
        {
            if (moveVer < 0)
            {
                if (playerPos + nodeManager.width < nodeManager.nodes.Length)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos + nodeManager.width].transform.position;
                    posToUpdate += nodeManager.width;
                }
            }
            else if (moveVer > 0)
            {
                if (playerPos - nodeManager.width >= 0)
                {
                    playerNodeEnd = nodeManager.nodes[playerPos - nodeManager.width].transform.position;
                    posToUpdate -= nodeManager.width;
                }
            }
            direction = Direction.VERTICAL;
            futurePlayerPos = playerPos + posToUpdate;
            return true;
        }
        return false;
    }

    void UpdatePlayerPos()
    {
        playerPos += posToUpdate;
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
       StartCoroutine(MovePlayerInManhole(nearManHole.transform.position, nearManHole.GetComponent<Manhole>().CouplePos(), nearManHole.GetComponent<Manhole>().couple.GetComponent<Manhole>().CheckNearNode(nearManHole.GetComponent<Manhole>().couple.transform.position)));
       isInHole = true;
       isinHacking = false;
    }

    IEnumerator MovePlayerInManhole(Vector3 startPos, Vector3 targetPos, int node)
    {
        GetComponent<SpriteRenderer>().color = Color.clear;
        float time = 0;
        float duration = 2f;

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
        GetComponent<SpriteRenderer>().color = Color.white;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        playerPos = node;
        isInHole = false;
        isAtNode = true;
        
    }

    IEnumerator WinGame()
    {
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("Vertical", 1);
        cam.transform.parent = null;
        Vector3 startVec = transform.position;
        float time = 0;
        float duration = 3f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startVec, finishFar.transform.position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

}
