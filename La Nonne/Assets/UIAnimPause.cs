using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Utils;
using DG.Tweening;
using UnityEngine;

public class UIAnimPause : MonoSingleton<UIAnimPause>
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject pauseMenu;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Jiggle()
    {
        pauseMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }

    public void OpenMenu()
    {
        if (!menuOpen)
        {
            anim.SetBool("isPauseOpen", true);
            menuOpen = true;
        }
    }

    public void CloseMenu()
    {
        if (menuOpen)
        {
            anim.SetBool("isPauseOpen", false);
            menuOpen = false;
        }
    }
}
