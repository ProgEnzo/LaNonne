using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiAnimScoreDetails : MonoBehaviour
{
    public Animator anim;
        private bool menuOpen = false;
        public GameObject detailsScoreMenu;
    
        public void Jiggle()
        {
            detailsScoreMenu.transform.DOShakePosition(0.2f, Vector3.one * 10, 10, 90f);
        }
    
        public void OpenMenu()
        {
            if (!menuOpen)
            {
                anim.SetBool("isScoreDetailsOn", true);
                menuOpen = true;
            }
        }
    
        public void CloseMenu()
        {
            if (menuOpen)
            {
                anim.SetBool("isScoreDetailsOn", false);
                menuOpen = false;
            }
        }
}
