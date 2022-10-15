using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDI : MonoBehaviour
{
    [Header("Enemy Health")] 
    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth;
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TDIDie();
        }
    }

    private void TDIDie()
    {
        Destroy(gameObject);
    }
}
