using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Utils;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    internal new static AudioManager instance;

    public AudioSource playerAudioSource;
    public AudioSource enemyAudioSource;
    public AudioSource UIAudioSource;
    public AudioSource shopAudioSource;
    public AudioSource musicAudioSource;
    public AudioSource bossAudioSource;
    public AudioSource othersAudioSource;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
