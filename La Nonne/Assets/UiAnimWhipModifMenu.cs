using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiAnimWhipModifMenu : MonoBehaviour
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject whipModifMenu;
    
    public void Jiggle()
    {
        whipModifMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }
    
    public void OpenMenu()
    {
        if (!menuOpen)
        { 
            anim.SetBool("isWhipModifMenuOpen", true); 
            menuOpen = true;
        }
    }
    
    public void CloseMenu()
    {
        if (menuOpen) 
        {
            anim.SetBool("isWhipModifMenuOpen", false);
            menuOpen = false;
        }
    }
}
