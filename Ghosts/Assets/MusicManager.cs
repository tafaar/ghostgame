using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Linq;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public Sound[] sounds;
    [Space(50)]
    public Sound[] music;

    List<Sound> musicQueue;
    float _songLength;
    float _songTimer;


    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Multiple music managers found");

            Destroy(gameObject);
        }
        instance = this;

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        musicQueue = music.ToList();

        Sound firstSong = musicQueue[UnityEngine.Random.Range(0, music.Length)];

        Play(firstSong.name, true);

        _songLength = firstSong.clip.length;

        musicQueue.Remove(firstSong);
    }

    public void Update()
    {
        _songTimer += Time.deltaTime;

        if(_songTimer >= _songLength)
        {
            _songTimer = 0;

            Sound nextSong = musicQueue[UnityEngine.Random.Range(0, musicQueue.Count - 1)];

            Play(nextSong.name, true);

            _songLength = nextSong.clip.length;

            musicQueue.Remove(nextSong);

            if (musicQueue.Count == 0)
            {
                musicQueue = music.ToList();
            }
        }
    }

    public void Play (string name, bool song = false)
    {
        if (!song)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.Log(name + " not found");
                return;
            }
            Debug.Log("Playing sound " + name);
            s.source.Play();
        }
        else
        {
            Sound s = Array.Find(music, sound => sound.name == name);
            if (s == null)
            {
                Debug.Log(name + " not found");
                return;
            }
            Debug.Log("Playing song " + name);
            s.source.Play();
        }
    }
}
