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
    float changeFrequencyTime = 3.0f;
    float changeFreqTime = 0;

    SerialPort stream = new SerialPort("COM3", 9600);

    void Start()
    {
        stream.ReadTimeout = 50;
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
        int valueInt = int.Parse(value);
        Debug.Log(valueInt);
        //if (index > 100) index = 100;
        //else if (index < 0) index = 0;
        if (Input.GetKey(KeyCode.LeftArrow) && index > 0) index -= speed;
        if (Input.GetKey(KeyCode.RightArrow) && index < 100) index += speed;

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
