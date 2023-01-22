using System.Collections;
using System.Collections.Generic;
using Controller;
using Manager;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    private InputManager inputManager;
    
    [Header("SoundEffect")] 
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepAudioClip;
    void Start()
    {
        inputManager = InputManager.instance;
    }

    void walkPlayerSound()
    {
        if (Input.GetKey(inputManager.leftMoveKey) || Input.GetKey(inputManager.rightMoveKey) || Input.GetKey(inputManager.upMoveKey) || Input.GetKey(inputManager.downMoveKey))
        {
            footstepAudioSource.PlayOneShot(footstepAudioClip[Random.Range(0, footstepAudioClip.Length)]);
        }
    }

    void walkBossSound()
    {
        footstepAudioSource.PlayOneShot(footstepAudioClip[Random.Range(0, footstepAudioClip.Length)]);
    }
}
