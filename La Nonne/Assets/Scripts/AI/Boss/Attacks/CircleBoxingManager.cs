using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;

public class CircleBoxingManager : MonoBehaviour
{
    public int circleBoxingDamage;
    public float pushForce;
    
    public GameObject player;
    public GameObject boss;
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        boss = GameObject.Find("BOSS");
        
    }
    private void OnTriggerEnter2D(Collider2D col)
    {       
        Vector2 direction = (player.transform.position - transform.position).normalized;

        if (col.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.TakeDamage(circleBoxingDamage);
            player.GetComponent<Rigidbody2D>().AddForce(direction * pushForce, ForceMode2D.Impulse);

        }
    }
    
    
}
