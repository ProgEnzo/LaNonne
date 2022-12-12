using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ShrinkingCircleManager : MonoBehaviour
{
    public GameObject player;
    public GameObject boss;

    public float pullForce;
    public float blackHoleRange;
    

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");

    }
    
    private void OnTriggerStay2D(Collider2D col)
    {
        Vector2 direction = (transform.position - player.transform.position).normalized;
        var distanceBossPlayer = Vector2.Distance(boss.transform.position, player.transform.position);

        if (distanceBossPlayer < blackHoleRange && PlayerController.instance.timerDash < 0)
        {
            player.GetComponent<Rigidbody2D>().AddForce(direction * pullForce, ForceMode2D.Impulse);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(boss.transform.position, blackHoleRange);
    }
    
    //DRAW LE BLACK HOLE RANGE ET VOIR COMMENT CA SE FAIT AVEC LE SPRITE RENDERER CIRCLE SHRINKING
}
