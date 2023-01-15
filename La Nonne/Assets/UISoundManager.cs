using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    [Header("SoundEffect")]
    public AudioSource buttonAudioSource;
    public AudioClip buttonClickAudioClip;
    public AudioClip buttonHoverAudioClip;
    public AudioClip buttonBackClickAudioClip;

    public void SoundOnClick()
    {
        buttonAudioSource.PlayOneShot(buttonClickAudioClip);
    }

    public void SoundOnHover()
    {
        buttonAudioSource.PlayOneShot(buttonHoverAudioClip);
    }

    public void SoundOnBackClick()
    {
        buttonAudioSource.PlayOneShot(buttonBackClickAudioClip);
    }
}
