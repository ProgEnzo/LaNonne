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
   
    private float distanceToPlayer;
    public float shootingRange;
    public float timeBetweenShots;
    public float bulletSpeed;
    private Vector3 currentPlayerPosition;
    private Vector3 currentEnemyPosition;
    
    
    [Header("Enemy Components")]
    public GameObject player;
    private AIPath scriptAIPath;
    public GameObject bullet;
    

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
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position); //Ca chope la distance entre le joueur et l'enemy
        
        if (distanceToPlayer <= shootingRange) //si le joueur EST dans la range de tir du trashMob
        {
            scriptAIPath.maxSpeed = 0;
            StartCoroutine(Shoot());

        }
        else if (distanceToPlayer >= shootingRange) //si le joueur N'EST PAS dans la range de tir du trashMob
        {
            scriptAIPath.maxSpeed = 10;
        }
    }

    private IEnumerator Shoot()
    {
        
        yield return new WaitForSeconds(timeBetweenShots);
        Instantiate(bullet, transform.position, Quaternion.identity);
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    
}
