using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    [Header("SoundEffect")] 
    public AudioSource bladeAudioSource;
    public AudioClip[] walkSound;
    void Start()
    {
        
    }

    void walkPlayerSound()
    {
        bladeAudioSource.PlayOneShot(walkSound[Random.Range(0, walkSound.Length)]);
    }
}
