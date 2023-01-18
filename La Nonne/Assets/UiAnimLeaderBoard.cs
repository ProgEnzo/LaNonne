using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiAnimLeaderBoard : MonoBehaviour
{
    public Animator anim;
    private bool menuOpen = false;
    public GameObject leaderBoardMenu;
       
    public void Jiggle()
    {
        leaderBoardMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
    }
       
    public void OpenMenu()
    {
        if (!menuOpen)
        { 
            anim.SetBool("isLeaderBoardOpen", true); 
            menuOpen = true;
        }
    }
       
    public void CloseMenu()
    {
        if (menuOpen) 
        {
            anim.SetBool("isLeaderBoardOpen", false);
            menuOpen = false;
        }
    }
}
