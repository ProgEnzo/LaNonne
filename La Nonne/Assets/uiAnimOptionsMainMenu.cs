using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class uiAnimOptionsMainMenu : MonoBehaviour
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
            anim.SetBool("isMenuOptionsOpen", true);
            menuOpen = true;
        }
    }

    public void CloseMenu()
    {
        if (menuOpen)
        {
            anim.SetBool("isMenuOptionsOpen", false);
            menuOpen = false;
        }
    }
}
