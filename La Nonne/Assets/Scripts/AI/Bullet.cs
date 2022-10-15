using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private int bulletDamage;

    private playerController playerController;
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Si les bullet du TrashMobRange touchent le player
        if (col.gameObject.CompareTag("Player"))
        {
            player.GetComponent<playerController>().TakeDamage(bulletDamage);
            Destroy(gameObject);
            Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, FOR THIS AMOUNT OF DMG : " + bulletDamage);
            
        }
    }

 
}
