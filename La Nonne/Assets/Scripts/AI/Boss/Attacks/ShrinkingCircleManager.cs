using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ShrinkingCircleManager : MonoBehaviour
{
    public GameObject player;
    public GameObject boss;

    public float pushForce;
    public float blackHoleRange;



    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
    }

    private void Update()
    {
        Vector2 direction = (transform.position - player.transform.position).normalized;
        var distanceBossPlayer = Vector2.Distance(boss.transform.position, player.transform.position);

        if (distanceBossPlayer < blackHoleRange && PlayerController.instance.m_timerDash < 0)
        {
            player.GetComponent<Rigidbody2D>().AddForce(direction * pushForce, ForceMode2D.Impulse);

        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(boss.transform.position, blackHoleRange);
    }
    
    //DRAW LE BLACK HOLE RANGE ET VOIR COMMENT CA SE FAIT AVEC LE SPRITE RENDERER CIRCLE SHRINKING
}
