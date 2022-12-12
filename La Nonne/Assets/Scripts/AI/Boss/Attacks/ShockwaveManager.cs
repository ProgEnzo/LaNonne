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


    void Start()
    {
        player = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            player.GetComponent<Rigidbody2D>().AddForce(direction * shockwaveForce, ForceMode2D.Impulse);
        }
    }
}
