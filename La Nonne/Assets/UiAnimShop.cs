using System.Collections;
using System.Collections.Generic;
using Core.Scripts.Utils;
using DG.Tweening;
using UnityEngine;

public class UiAnimShop : MonoSingleton<UIAnimPause>
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject shopMenu;

    public void Jiggle()
    {
        shopMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }

    public void OpenMenu()
    {
        if (!menuOpen)
        {
            anim.SetBool("isShopOpen", true);
            menuOpen = true;
        }
    }

    public void CloseMenu()
    {
        if (menuOpen)
        {
            anim.SetBool("isShopOpen", false);
            menuOpen = false;
        }
    }
}
