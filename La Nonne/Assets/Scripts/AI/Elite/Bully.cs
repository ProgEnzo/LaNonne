using System;
using System.Collections;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Elite
{
    public class Bully : MonoBehaviour
    {
        [Header("Enemy Health")] 
        [SerializeField] public float currentHealth;
    
        [Header("Enemy Attack")]
        [SerializeField] private int bullyDamage;
        [SerializeField] private float knockbackPower;

        [Header("Enemy Components")]
        public PlayerController playerController;
        
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;
    
        public bool isStunned;


        private void Start()
        {
            currentHealth = soEnemy.maxHealth;
            isStunned = false;
        }

        private void Awake()
        {
            //Assignation du script au prefab ON SPAWN
            playerController = PlayerController.instance;
            GetComponent<AIDestinationSetter>().target = playerController.transform;
        }

        private void Update()
        {
            HealCeiling();
        }

        #region HealthEnemyClose
        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                BullyDie();
            }
        }
    
        private void BullyDie()
        {
            Destroy(gameObject);
        }

        private void HealCeiling()
        {
            if (currentHealth > soEnemy.maxHealth)
            {
                currentHealth = soEnemy.maxHealth;
            }
        }
    
        #endregion
        

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(bullyDamage); //Player takes damage

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * knockbackPower;
            
                playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }


        IEnumerator PlayerIsHit()
        {
            playerController.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerController.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
