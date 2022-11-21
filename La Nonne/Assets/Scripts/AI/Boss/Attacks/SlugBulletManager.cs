using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class SlugBulletManager : MonoBehaviour
{
    public int bulletDamage;
    private Rigidbody2D rb;
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(bulletDamage);
        }
    }
}
