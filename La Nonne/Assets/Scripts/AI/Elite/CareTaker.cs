using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private int caretakerBodyDamage;
        [SerializeField] private int circleDamage;
        [SerializeField] private float knockbackPower;
        [SerializeField] private float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;

        [Header("Enemy Components")]
        public PlayerController playerController;
        public SO_Controller soController;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleDamageSprite;
        [SerializeField] private GameObject circleHealSprite;

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;
        public List<GameObject> y;
    
        public bool isStunned;


    

        private void Start()
        {
            currentHealth = soEnemy.maxHealth;
            GoToTheNearestMob();
            
            //Zones de heal / dégâts
            circle.enabled = false;
            circleDamageSprite.SetActive(false);
            circleHealSprite.SetActive(false);
        }

        private void Update()
        {
            if (!isStunned)
            {
                gameObject.GetComponent<AIDestinationSetter>().enabled = true;
                CheckIfTargetIsDead();

                //Timer for spawning circles
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer > 0)
                {
                    return;
                }

                cooldownTimer = timeBetweenCircleSpawn;
                
                StartCoroutine(BlinkCircle());
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
                if (col.gameObject.CompareTag("Bully") || col.gameObject.CompareTag("TrashMobClose") ||
                    col.gameObject.CompareTag("TrashMobRange"))
                {

                }

                if (col.gameObject.CompareTag("Player"))
                {
                    

                    

                    playerController.TakeDamage(circleDamage); //Player takes damage

                }
            }
        }

        IEnumerator BlinkCircle()
        {
            circle.enabled = true;
            circleDamageSprite.SetActive(true);
            yield return new WaitForSeconds(1);
            circle.enabled = false;
            circleDamageSprite.SetActive(false);

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
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(caretakerBodyDamage); //Player takes damage

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
            playerController.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    
    }
}
