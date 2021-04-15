using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Radio : MonoBehaviour
{
    public GameObject minPos, maxPos, pos;
    float index;
    Vector3 minVec, maxVec;
    float speed;
    public bool isOnFrequency = false;
    float freqAccuracy;
    float frequency, minBound, maxBound;
    float changeFrequencyTime = 7.0f;
    float changeFreqTime = 0;

    public TextMesh freqText;
    public GameObject button;

    SerialPort stream = new SerialPort("COM3", 9600);

    void Start()
    {
        //stream.ReadTimeout = 50;
        stream.Open();


        minVec = minPos.transform.position;
        maxVec = maxPos.transform.position;
        speed = 10 * Time.deltaTime;
        changeFreqTime = changeFrequencyTime;
        RandomizeFrequency();
    }

    void Update()
    {

        string value = stream.ReadLine();
        float valueInt = float.Parse(value.Split('_')[0]);
        if (valueInt <= 800)
        {
            index = (valueInt / 800) * 100;
            freqText.text = index.ToString();
            button.transform.rotation = Quaternion.Euler(0, 0, valueInt);
        }

        Debug.Log(frequency);
        pos.transform.position = MovePos();

        if (index >= minBound && index <= maxBound)
            isOnFrequency = true;
        else
            isOnFrequency = false;

        changeFreqTime -= Time.deltaTime;
        if (changeFreqTime <= 0)
        {
            RandomizeFrequency();
            changeFreqTime = changeFrequencyTime;
        }
            
    }

    Vector3 MovePos()
    {
        return Vector3.Lerp(minVec, maxVec, index / 100);
    }

    void RandomizeFrequency()
    {
        frequency =  Random.Range(0, 100);
        minBound = frequency - 5;
        maxBound = frequency + 5;
    }


}
