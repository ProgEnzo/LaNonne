using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class GrowingCircleManager : MonoBehaviour
{
    public BossStateManager bossScript;
    public PlayerController player;

    public float pushForce;

    private void Awake()
    {
        //Assignation du script au prefab ON SPAWN
        player = PlayerController.instance;
        
        bossScript = GetComponent<BossStateManager>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 direction = (bossScript.transform.position - player.transform.position).normalized;     
        
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("IAGIHOAEHOAEGOAUEGHOAEUGHPAGHPAGHAEg");
           player.m_rigidbody.AddForce(direction * pushForce, ForceMode2D.Impulse);
        }
    }
}
