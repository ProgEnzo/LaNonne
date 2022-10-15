using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashMobClose : MonoBehaviour
{
    [Header("Enemy Health")] 
    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth;
    
    [Header("Enemy Attack")]
    [SerializeField] private int trashMobCloseDamage;

    [Header("Enemy Components")]
    public playerController playerController;


    private void Start()
    {
        currentHealth = maxHealth;
    }

    #region HealthEnemyClose
    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TrashMobCloseDie();
        }
    }
    
    private void TrashMobCloseDie()
    {
        Destroy(gameObject);
    }
    
    #endregion
    

    private void OnCollisionEnter2D(Collision2D col) 
    {
        //Si le TrashMobClose touche le player
        if (col.gameObject.CompareTag("Player"))
        {
            playerController.TakeDamage(trashMobCloseDamage); //Player takes damage
        }
    }
    
}
