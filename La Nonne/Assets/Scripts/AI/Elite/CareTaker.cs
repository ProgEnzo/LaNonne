using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class CareTaker : MonoBehaviour
{
    [Header("Enemy Health")] 
    [SerializeField] public float currentHealth;
    
    [Header("Enemy Attack")]
    [SerializeField] private int caretakerBodyDamage;
    [SerializeField] private int circleDamage;
    [SerializeField] private float knockbackPower;
    [SerializeField] private float cooldownTimer;
    [SerializeField] private float timeBetweenCircleSpawn;

    [Header("Enemy Components")]
    public playerController playerController;
    private CircleCollider2D circle;
    public SO_Controller soController;

    public SO_Enemy SO_Enemy;
    public List<GameObject> y;


    

    private void Start()
    {
        currentHealth = SO_Enemy.maxHealth;
        GoToTheNearestMob();
    }

    private void Update()
    {
        CheckIfTargetIsDead();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bully") || col.gameObject.CompareTag("TrashMobClose") || col.gameObject.CompareTag("TrashMobRange"))
        {
            
        }

        if (col.gameObject.CompareTag("Player"))
        {
            //Timer for firing bullets
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0)
            {
                return;
            }
            cooldownTimer = timeBetweenCircleSpawn;
            
            playerController.TakeDamage(circleDamage); //Player takes damage

        }
    }

    public void GoToTheNearestMob()
    {
        // y = liste de tous les gameobjects mobs/enemy
        
        //tous les gameobjects de la scène vont se mettre dans "y", transforme le dico en liste (parce qu'on peut pas modif les valeurs du dico)
        y = FindObjectsOfType<GameObject>().ToList(); 
        

        foreach (var gObject in y.ToList()) //pour chaque gObject dans y alors exécute le script
        {
            //exécuter ce script pour tous les game objects sauf ces mobs 
            if (!gObject.CompareTag("Bully") && !gObject.CompareTag("TrashMobClose") && !gObject.CompareTag("TrashMobRange"))
            {
                y.Remove(gObject);
            }
        }
        
        y = y.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
        
        //prendre l'enemy le plus proche en new target (le premier de la liste)
        if (y[0])
        {
            GetComponent<AIDestinationSetter>().target = y[0].transform;
        }
    }
    void CheckIfTargetIsDead()
    {
        if (GetComponent<AIDestinationSetter>().target == null)
        {
            GoToTheNearestMob();
        }
    }

    #region HealthEnemyClose
    public void TakeDamageFromPlayer(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            TrashMobCloseDie();
        }
    }
    
    
    
    private void TrashMobCloseDie()
    {
        Destroy(gameObject);
    }
    
    #endregion
    

    private void OnCollisionEnter2D(Collision2D col) 
    {
        //Si le caretaker touche le player
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayerIsHit());
            playerController.TakeDamage(caretakerBodyDamage); //Player takes damage

            Collider2D collider2D = col.collider; //the incoming collider2D (celle du player en l'occurence)
            Vector2 direction = (collider2D.transform.position - transform.position).normalized;
            Vector2 knockback = direction * knockbackPower;
            
            playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
        }
    }


    IEnumerator PlayerIsHit()
    {
        playerController.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        playerController.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    
}
