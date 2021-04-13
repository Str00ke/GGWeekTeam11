using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_Code : MonoBehaviour
{
    NodesManager nodeManager;
    MiniGameManager miniGameManager;
    public GameObject CodeShowPanel, CodeGuessPanel;
    public Text codeShow;
    public Text[] codeGuessArr = new Text[4];
    public Text[] codeGuessList = new Text[10];
    int nbr;
    string code;
    int codeToGuess;
    int codeIndex;
    // Start is called before the first frame update
    void Start()
    {
        nodeManager = FindObjectOfType<NodesManager>();
        miniGameManager = FindObjectOfType<MiniGameManager>();
        for (int i = 0; i < 4; ++i)
        {
            code += RandomizeNbr(i).ToString();
            codeGuessArr[i].text = 0.ToString();
        }
        codeShow.text = code;
        codeToGuess = int.Parse(code);

        for (int i = 0; i < 10; ++i)
        {
            codeGuessList[i].text = i.ToString();
        }
    }

    void Update()
    {

    }

    int RandomizeNbr(int index)
    {
        return index == 0 ? Random.Range(1, 10) : Random.Range(0, 10);
    }

    public void PressButton(int value)
    {
        if (codeIndex < 4)
        {
            codeGuessArr[codeIndex].text = value.ToString();
            codeIndex++;
            if (codeIndex == 4)
                StartCoroutine(Tempo());
        }
            
    }

    void CheckCode(Text[] guess)
    {
        int tempo = 2;
        float index = 0;
        while(index < tempo)
        {
            index += Time.deltaTime;
        }

        string finalCode = "";
        int code;
        for (int i = 0; i < 4; ++i)
        {
            finalCode += guess[i].text;
        }
        code = int.Parse(finalCode);
        if (code == codeToGuess)
        {
            miniGameManager.Success();
            Destroy(gameObject);
        }           
        else
            ResetCode();
    }

    IEnumerator Tempo()
    {
        yield return new WaitForSeconds(0.5f);
        CheckCode(codeGuessArr);
    }

    void ResetCode()
    {
        Debug.Log("NAAAAAAAAA");
        for (int i = 0; i < 4; ++i)
        {
            codeGuessArr[i].text = 0.ToString();
        }
        codeIndex = 0;
    }
}
