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
    [SerializeField] float timeBetweenShots;
    [SerializeField] float bulletSpeed;
    private Vector3 currentPlayerPosition;
    private Vector3 currentEnemyPosition;
    
    
    [Header("Enemy Components")]
    
    [SerializeField] Transform player;
    private AIPath scriptAIPath;
    [SerializeField] GameObject bullet;
    

    private void Start()
    {
        scriptAIPath = GetComponent<AIPath>();
        currentPlayerPosition = player.transform.position;
        currentEnemyPosition = transform.position;


    }
    private void Update()
    {
        StopAndShoot();
    }

    void StopAndShoot()
    {
        distanceToPlayer = Vector2.Distance(player.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
        if (distanceToPlayer <= shootingRange) //si le joueur EST dans la shootingRange du trashMob
        {
            scriptAIPath.maxSpeed = 6;
            
            Debug.Log("DANS LA SHOOTING RANGE");
            Debug.Log(distanceToPlayer);
        }
        else if (distanceToPlayer <= aggroRange) //si le joueur EST dans l'aggroRange du trashMob
        {
            scriptAIPath.maxSpeed = 3;
            StartCoroutine(Shoot());
            
            Debug.Log("DANS LAGGRO RANGE");
        }
        
        
    }

    private IEnumerator Shoot()
    {

        yield return new WaitForSeconds(timeBetweenShots);
        //Instantiate(bullet, currentEnemyPosition, Quaternion.identity);

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
    
}
