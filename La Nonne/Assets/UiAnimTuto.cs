using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiAnimTuto : MonoBehaviour
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject tutoMenu;

    public void Jiggle()
    {
        tutoMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }

    public void OpenMenu()
    {
        if (!menuOpen)
        {
            anim.SetBool("isTutoOpen", true);
            menuOpen = true;
        }
    }

    public void CloseMenu()
    {
        if (menuOpen)
        {
            anim.SetBool("isTutoOpen", false);
            menuOpen = false;
        }
    }
}
