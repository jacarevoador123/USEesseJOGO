using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Audio[] audios;

    public static AudioManager Instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Audio audio in audios)
        {
            audio.source = gameObject.AddComponent<AudioSource>();
            audio.source.clip = audio.clip;

            audio.source.volume = audio.volume;
            audio.source.pitch = audio.pitch;
            audio.source.loop = audio.loop;

        }
    }

    public void Play(string name)
    {
        Audio audio = Array.Find(audios, audio => audio.name == name);

        if (audio == null) return;

        audio.source.Play();
    }

    void Start()
    {
        Play("Tema");
    }
}
