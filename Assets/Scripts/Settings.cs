using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject soundOn;
    [SerializeField] GameObject soundOff;
    float volume = 0;
    bool isMute = false;
 
    public void getVolume (float v)
    {
        volume = v;
        if (!isMute)
        {
            SetVolume(volume);
        }
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;     
    }

    public void Back()
    {
        menu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Mute(bool m)
    {
        isMute = m;
        soundOff.SetActive(isMute);
        soundOn.SetActive(!isMute);

        if (isMute)
        {
            SetVolume(-80);
        }
        else
        {
            SetVolume(volume);
        }
    }

    private void SetVolume (float v)
    {
        audioMixer.SetFloat("volume", v);
        Debug.Log(v);
    }

}
