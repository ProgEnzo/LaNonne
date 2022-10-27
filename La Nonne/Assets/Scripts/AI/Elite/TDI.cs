using System;
using System.Collections;
using AI.Trash;
using Controller;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace AI.Elite
{
    public class TDI : MonoBehaviour
    {
        [Header("Enemy Attack")] 
        [SerializeField] public int bodyDamage;
        [SerializeField] public int bodyKnockback;
        [SerializeField] public int circleDamage;
        
        
        [SerializeField] public float distanceToPlayer;
        [SerializeField] public float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;


        
        [Header("Enemy Health")] 
        [SerializeField] public float currentHealth;
        [SerializeField] public float maxHealth;
        [SerializeField] public float healAmount;
        
        [Header("Components")] 
        [SerializeField] public GameObject bully;
        [SerializeField] public GameObject caretaker;
        [SerializeField] public GameObject player;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;
        public PlayerController playerController;
        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Awake()
        {
            //Assignation du script au prefab ON SPAWN
            playerController = PlayerController.instance;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            //Heal le TrashMobCLose
            if (col.gameObject.CompareTag("TrashMobClose"))
            {
                col.gameObject.GetComponent<TrashMobClose>().currentHealth += healAmount;
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }

            //Heal le TrashMobRange
            if (col.gameObject.CompareTag("TrashMobRange"))
            {
                col.gameObject.GetComponent<TrashMobRange>().currentHealth += healAmount;
                // Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //Heal le Bully
            if (col.gameObject.CompareTag("Bully"))
            {
                col.gameObject.GetComponent<Bully>().currentHealth += healAmount;
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //Heal le Caretaker
            if (col.gameObject.CompareTag("Caretaker"))
            {
                col.gameObject.GetComponent<CareTaker>().currentHealth += healAmount;
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }

            if (col.gameObject.CompareTag("Player"))
            {
                col.GetComponent<PlayerController>().TakeDamage(circleDamage); //Player takes damage
                StartCoroutine(PlayerIsHit());
            }
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player"))
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(bodyDamage); //Player takes damage

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * bodyKnockback;
            
                playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
        
        IEnumerator PlayerIsHit()
        {
            playerController.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerController.GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 50)
            {
                StartCoroutine(DieAndSpawn());
            }
        }
        
        IEnumerator DieAndSpawn()
        {
            transform.DOScale(new Vector3(3, 0, 3), 0.1f);
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
            Instantiate(bully, transform.position, quaternion.identity);
            Instantiate(caretaker, transform.position, quaternion.identity);
            
        }
        
        void DistanceBetweenPlayer()
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
            //Timer for spawning circles
            cooldownTimer -= Time.deltaTime; // cooldowntimer = -1 toutes les secondes
            if (cooldownTimer > 0)
            {
                return;
            }

            cooldownTimer = timeBetweenCircleSpawn;
            StartCoroutine(BlinkCircle());
        }

        IEnumerator BlinkCircle()
        {
            circle.enabled = true;
            circleSprite.SetActive(true);
            currentHealth += healAmount;
            yield return new WaitForSeconds(2);
            circle.enabled = false;
            circleSprite.SetActive(false);
        }
    }
}
