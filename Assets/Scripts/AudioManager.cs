using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] sounds;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        foreach (Audio s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            if (s.clips.Length > 1)
            {
                s.hasMultipleClips = true;
            }
            else
            {
                s.source.clip = s.clips[0];
                s.source.loop = s.loop;
            }          

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play (string name)
    {
        Audio s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            return;

        if (s.hasMultipleClips)
        {
            int newIndex = UnityEngine.Random.Range(0, s.clips.Length);
            s.source.clip = s.clips[newIndex];
        }
        else
        {
            s.source.Play();
        }
    }
    
    public void Stop (string name)
    {
        Audio s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            return;

        s.source.Stop();
    }

    public void Pause (string name)
    {
        Audio s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            return;

        s.source.Pause();
    }
}
