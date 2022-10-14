using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class TrashMobRange : MonoBehaviour
{
    [Header("Enemy Detection")]
    [SerializeField] float distanceToPlayer;
    [SerializeField] float shootingRange;
    [SerializeField] float aggroRange;
    private float cooldownTimer;
    
    [Header("Enemy Attack Values")]
    [SerializeField] float cooldownBetweenShots;
    [SerializeField] float bulletSpeed;
    [SerializeField] float damageTaken;

    [Header("Enemy Components")]
    [SerializeField] GameObject player;
    private AIPath scriptAIPath;
    [SerializeField] GameObject bulletPrefab;

    [Header("Enemy Health")] 
    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth;
    

    private void Start()
    {
        scriptAIPath = GetComponent<AIPath>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        StopAndShoot();
    }

    void StopAndShoot()
    {
        distanceToPlayer = Vector2.Distance(player.transform.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
        if (distanceToPlayer <= shootingRange) //si le joueur est dans la SHOOTING RANGE du trashMob
        {
            scriptAIPath.maxSpeed = 3;
            Shoot();
            
        }
        else if (distanceToPlayer <= aggroRange) //si le joueur est dans l'AGGRO RANGE du trashMob
        {
            scriptAIPath.maxSpeed = 6;
            
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

       
        Vector3 direction = player.transform.position - transform.position;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);
        
        Destroy(bullet, 4f);

        

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
    
    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TrashMobRangeDie();
        }
    }

    private void TrashMobRangeDie()
    {
        Destroy(gameObject);
    }
}
