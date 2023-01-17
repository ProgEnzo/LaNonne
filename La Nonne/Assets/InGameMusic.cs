using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusic : MonoBehaviour
{
    [Header("SoundEffect")] 
    public AudioSource musicAudioSource;
    public AudioClip musicAudioClip;
    
    void Start()
    {
        StartCoroutine(PlayAndRepeatMusic());
    }

    private IEnumerator PlayAndRepeatMusic()
    {
        musicAudioSource.PlayOneShot(musicAudioClip);
        yield return new WaitForSeconds(musicAudioClip.length);

        StartCoroutine(PlayAndRepeatMusic());
    }
}
