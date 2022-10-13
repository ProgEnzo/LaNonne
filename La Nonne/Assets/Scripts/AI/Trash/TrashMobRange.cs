using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class TrashMobRange : MonoBehaviour
{
    [Header("Enemy Values")]
   
    [SerializeField] float distanceToPlayer;
    [SerializeField] float shootingRange;
    [SerializeField] float aggroRange;
    [SerializeField] float cooldownBetweenShots;
    [SerializeField] float cooldownTimer;
    [SerializeField] float bulletSpeed;
    
    [Header("Enemy Components")]
    
    [SerializeField] Transform player;
    private AIPath scriptAIPath;
    [SerializeField] GameObject bullet;
    [SerializeField] Rigidbody2D bulletRigidbody2D;
    

    private void Start()
    {
        scriptAIPath = GetComponent<AIPath>();
    }
    private void Update()
    {
        StopAndShoot();
    }

    void StopAndShoot()
    {
        distanceToPlayer = Vector2.Distance(player.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
        if (distanceToPlayer <= shootingRange) //si le joueur est dans la SHOOTING RANGE du trashMob and canShoot
        {
            scriptAIPath.maxSpeed = 3;
            Shoot();
            

            Debug.Log("DANS LA SHOOTING RANGE");
        }
        else if (distanceToPlayer <= aggroRange) //si le joueur est dans l'AGGRO RANGE du trashMob
        {
            scriptAIPath.maxSpeed = 6;
            
            Debug.Log("DANS LAGGRO RANGE");
        }
        else if (distanceToPlayer > aggroRange) //si le joueur est HORS de l'AGGRO RANGE
        {
            scriptAIPath.maxSpeed = 0;

        }
        
        
    }

    private void Shoot()
    {
        
        //Timer for firing bullets
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0)
        {
            return;
        }
        cooldownTimer = cooldownBetweenShots;
        
        
        Instantiate(bullet, transform.position, Quaternion.identity);
        bulletRigidbody2D.velocity = new Vector2(player.position.x, player.position.y) * bulletSpeed;

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
    
}
