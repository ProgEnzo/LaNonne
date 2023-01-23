using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    public void FadeOnClickStart()
    {
        musicAudioSource.DOFade(0f, 0.2f);
    }
}
