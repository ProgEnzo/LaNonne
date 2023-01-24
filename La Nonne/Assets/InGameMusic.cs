using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Manager;


public class InGameMusic : MonoBehaviour
{
    [Header("SoundEffect")] 
    public AudioSource musicAudioSource;
    public AudioSource ambianceAudioSource;
    public AudioClip musicAudioClip;
    public AudioClip[] ambianceAudioClip;
    
    void Start()
    {
        StartCoroutine(PlayAndRepeatMusic());
        StartCoroutine(RandomAmbianceSound());

    }

    private IEnumerator PlayAndRepeatMusic()
    {
        yield return new WaitForSeconds(10f); //temps de l'intro

        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);
        
    }


    private IEnumerator RandomAmbianceSound()
    {
        float delay = UnityEngine.Random.Range(1f, 10f);
        yield return new WaitForSeconds(delay);

        ambianceAudioSource.PlayOneShot(ambianceAudioClip[UnityEngine.Random.Range(0, ambianceAudioClip.Length)]);
        yield return new WaitForSeconds(ambianceAudioClip.Length);

        StartCoroutine(RandomAmbianceSound());

    }

}
