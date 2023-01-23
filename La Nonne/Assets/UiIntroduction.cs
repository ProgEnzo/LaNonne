using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class UiIntroduction : MonoBehaviour
{
      public Animator anim;
      private bool menuOpen = false;
      public GameObject intro;

      public AudioSource introAudioSource;
      public AudioClip introAudioClip1;
      public AudioClip introFinalAudioClip;
  
      public void Jiggle()
      {
          intro.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
      }
  
      public void OpenMenu()
      {
          if (!menuOpen)
          {
              anim.SetBool("isIntroOn", true);
              menuOpen = true;
          }
      }

      public void PlayRandomSound1()
      {
          introAudioSource.PlayOneShot(introAudioClip1);
      }
      public void LastAudioClip()
      {
          introAudioSource.PlayOneShot(introFinalAudioClip);

      }
}
