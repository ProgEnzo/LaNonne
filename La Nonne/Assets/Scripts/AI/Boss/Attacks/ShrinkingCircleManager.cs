using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class ShrinkingCircleManager : MonoBehaviour
{
    public GameObject player;

    public float pushForce;


    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        Vector2 direction = (transform.position - player.transform.position).normalized;     
        
        if (col.gameObject.CompareTag("Player") && PlayerController.instance.m_timerDash < 0)
        {
            player.GetComponent<Rigidbody2D>().AddForce(direction * pushForce, ForceMode2D.Impulse);
        }
    }
}
