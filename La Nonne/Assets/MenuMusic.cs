using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [Header("SoundEffect")]
    public AudioSource musicAudioSource;
    public AudioClip musicMenuAudioClip;

    private void Start()
    {
        musicAudioSource.clip = musicMenuAudioClip;
        musicAudioSource.Play();
    }
}
