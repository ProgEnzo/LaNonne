using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimLoadingScript : MonoBehaviour
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject isLoading;
  
    public void Jiggle()
    {
        isLoading.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }
  
    public void OpenMenu()
    {
        if (!menuOpen)
        {
            anim.SetBool("isLoading", true);
            menuOpen = true;
        }
    }
}
