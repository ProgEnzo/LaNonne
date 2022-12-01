using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using UnityEngine;

public class ShockwaveManager : MonoBehaviour
{
    public PlayerController player;
    public float shockwaveForce;
    private CircleCollider2D shockwaveCollider;


    void Start()
    {
        player = PlayerController.instance;
        shockwaveCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player.TakeDamage(20);
            Vector2 direction = (player.transform.position - transform.position).normalized;
            player.GetComponent<Rigidbody2D>().AddForce(direction * shockwaveForce, ForceMode2D.Impulse);
        }
    }
}
