using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashMobClose : MonoBehaviour
{
    [SerializeField] private int trashMobCloseDamage;
    public playerController playerController;
    
    

    private void OnCollisionEnter2D(Collision2D col) 
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.TakeDamage(trashMobCloseDamage); //Quand le trash mob close touche le joueur, le joueur prend des dégâts
        }
    }
    
}
