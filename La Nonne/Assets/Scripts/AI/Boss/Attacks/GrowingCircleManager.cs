using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class GrowingCircleManager : MonoBehaviour
{
    public GameObject boss;
    public GameObject player;

    public float pushForce;


    private void Awake()
    {
        boss = GameObject.FindWithTag("Boss");
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 direction = (player.transform.position - boss.transform.position).normalized;     
        
        if (col.gameObject.CompareTag("Player") && PlayerController.instance.m_timerDash < 0)
        {
            player.GetComponent<Rigidbody2D>().AddForce(direction * pushForce, ForceMode2D.Impulse);
            Debug.Log("IAGIHOAEHOAEGOAUEGHOAEUGHPAGHPAGHAEg");
        }
    }
}
