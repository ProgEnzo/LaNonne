using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.Trash;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Elite
{
    public class CareTaker : MonoBehaviour
    {
        [Header("Enemy Health")] 
        [SerializeField] public float currentHealth;
    
        [Header("Enemy Attack")]
        [SerializeField] private int circleDamage;
        [SerializeField] private int bodyDamage;
        [SerializeField] private int healAmount;
        [SerializeField] private float bodyKnockback;
        [SerializeField] private float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;
        [SerializeField] private float attackRange;
        [SerializeField] public bool blinkExecuted;

        [Header("Enemy Components")]
        public PlayerController playerController;
        public SO_Controller soController;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;
        [SerializeField] private GameObject player;

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;
        public List<GameObject> y;
    
        public bool isStunned;


    

        private void Start()
        {
            currentHealth = soEnemy.maxHealth;
            GoToTheNearestMob();
            
            //Zones de heal / dégâts
            circle.enabled = false;
            circleSprite.SetActive(false);
        }

        private void Awake()
        {
            //Assignation du script au prefab ON SPAWN
            playerController = PlayerController.instance;
            player = GameObject.FindWithTag("Player");

        }

        private void Update()
        {
            CircleTimer();
            HealCeiling();
            
            if (!isStunned)
            {
                gameObject.GetComponent<AIDestinationSetter>().enabled = true;
                CheckIfTargetIsDead();
               
            }
            else
            {
                gameObject.GetComponent<AIDestinationSetter>().enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            
            if (!isStunned)
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
        }
        
        void CircleTimer()
        {
            //Timer for spawning circles
            if (!blinkExecuted && cooldownTimer <= 0f)
            {
                blinkExecuted = true;
                StartCoroutine(BlinkCircle());
            }
            else
            {
                cooldownTimer -= Time.deltaTime; // cooldowntimer = -1 toutes les secondes
            }
        }

        IEnumerator BlinkCircle()
        {
            circle.enabled = true;
            circleSprite.SetActive(true);
            currentHealth += healAmount;
            yield return new WaitForSeconds(timeBetweenCircleSpawn);
            circle.enabled = false;
            circleSprite.SetActive(false);
            cooldownTimer = timeBetweenCircleSpawn;
            blinkExecuted = false;
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.DrawWireSphere(position, attackRange);
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
    }
}
