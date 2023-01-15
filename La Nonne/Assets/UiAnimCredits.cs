using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiAnimCredits : MonoBehaviour
{
   public Animator anim;
   private bool menuOpen = false;
   public GameObject creditsMenu;
       
       public void Jiggle()
       {
           creditsMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
       }
       
       public void OpenMenu()
       {
           if (!menuOpen)
           { 
               anim.SetBool("isCreditPlaying", true); 
               menuOpen = true;
           }
       }
       
       public void CloseMenu()
       {
           if (menuOpen) 
           {
               anim.SetBool("isCreditPlaying", false);
               menuOpen = false;
           }
       }
}
