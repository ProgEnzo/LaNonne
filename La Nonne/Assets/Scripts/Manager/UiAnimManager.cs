using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEngine;

public class UiAnimManager : MonoBehaviour
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject optionMenu;

    public void Jiggle()
    {
        optionMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }

    public void OpenMenu()
    {
        if (!menuOpen)
        {
            anim.SetBool("isMenuOpen", true);
            menuOpen = true;
        }
    }

    public void CloseMenu()
    {
        if (menuOpen)
        {
            UIManager.isSettingsOn = false;
            anim.SetBool("isMenuOpen", false);
            menuOpen = false;
        }
    }
}
