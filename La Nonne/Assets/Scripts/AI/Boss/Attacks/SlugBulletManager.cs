using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class SlugBulletManager : MonoBehaviour
{
    public int bulletDamage;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }

    
}
