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
    [SerializeField] private int caretakerbodyDamage;
    [SerializeField] private float knockbackPower;

    [Header("Enemy Components")]
    public playerController playerController;

    public SO_Enemy SO_Enemy;
    public List<GameObject> y;


    public void SetNewMobToHeal()
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
            SetNewMobToHeal();
        }
    }

    private void Start()
    {
        currentHealth = SO_Enemy.maxHealth;
        SetNewMobToHeal();
    }

    private void Update()
    {
        CheckIfTargetIsDead();
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
            playerController.TakeDamage(caretakerbodyDamage); //Player takes damage

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
