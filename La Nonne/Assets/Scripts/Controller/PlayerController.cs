using System.Collections;
using System.Collections.Generic;
using AI.Elite;
using AI.Trash;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public Rigidbody2D m_rigidbody;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
    
        [SerializeField] private float m_timerDash = 0f;

        public static PlayerController instance;

        public RoomFirstDungeonGenerator rfg;

        [Header("Revealing Dash")]
        public bool isHitting;
        public float hitSpeed = 1f;
        public float revealingDashDetectionRadius = 1f;
        public int revealingDashEpCost;
        public GameObject revealingDashAimedEnemy;
        public float toleranceDistance = 0.1f;
        public Vector3 newPosition;
        public float stunDuration = 1f;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instance = this;
            }
            m_rigidbody = GetComponent<Rigidbody2D>();
        }
    
    
        private void Start()
        {
            soController.currentHealth = soController.maxHealth;
            isHitting = false;
            ReInit();
        }

        public void ResetVelocity()
        {
            m_rigidbody.velocity = Vector2.zero;
        }

        public void ReInit()
        {
            transform.position = new Vector3(rfg.roomCenters[0].x, rfg.roomCenters[0].y, 0);
        }
    
        void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_timerDash < -0.5f)
            {
                m_timerDash = soController.m_durationDash;
            }
        
            m_timerDash -= Time.deltaTime;
        
            RevealingDash();

        }
        public void FixedUpdate()
        {
            m_rigidbody.drag = soController.dragDeceleration * soController.dragMultiplier;
            ManageMove();
        }
    
        #region MovementPlayer
        private void ManageMove()
        {
            var speed = m_timerDash <= 0 ? soController.m_speed : soController.m_dashSpeed;

            int nbInputs = (Input.GetKey(KeyCode.Z) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? 1 : 0) +
                           (Input.GetKey(KeyCode.S) ? 1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            if (nbInputs > 1) speed *= 0.75f;

            if (Input.GetKey(KeyCode.Z))
            {
                m_rigidbody.AddForce(Vector2.up*speed);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                m_rigidbody.AddForce(Vector2.left*speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_rigidbody.AddForce(Vector2.down*speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                m_rigidbody.AddForce(Vector2.right*speed);
            }
        }
    

        #endregion

        #region HealthPlayer
        public void TakeDamage(int damage)
        {
        
            soController.currentHealth -= damage;
            Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, HEALTH REMAINING : " + soController.currentHealth);

            if (soController.currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            //Destroy(gameObject);
            Debug.Log("<color=green>PLAYER</color> IS NOW DEAD");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    

        #endregion

        #region AttackPlayer
        void RevealingDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isHitting && soController.epAmount >= revealingDashEpCost)
            {
                List<RaycastHit2D> enemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, revealingDashDetectionRadius, Vector2.zero, new ContactFilter2D(), enemiesInArea);
                enemiesInArea.Sort((x, y) => x.distance.CompareTo(y.distance));
                foreach (RaycastHit2D enemy in enemiesInArea)
                {
                    if (enemy.collider.CompareTag("TrashMobClose") || enemy.collider.CompareTag("TrashMobRange") || enemy.collider.CompareTag("Bully") || enemy.collider.CompareTag("Caretaker"))
                    {
                        soController.epAmount -= revealingDashEpCost;
                        revealingDashAimedEnemy = enemy.collider.gameObject;
                        newPosition = revealingDashAimedEnemy.transform.position;
                        isHitting = true;
                        break;
                    }
                }
            }

            if (isHitting)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPosition, hitSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, newPosition) < toleranceDistance)
                {
                    StopCoroutine(StunEnemy(revealingDashAimedEnemy));
                    StartCoroutine(StunEnemy(revealingDashAimedEnemy));
                
                    //DMG du player sur le TrashMobClose
                    if (revealingDashAimedEnemy.CompareTag("TrashMobClose"))
                    {
                        revealingDashAimedEnemy.GetComponent<TrashMobClose>().TakeDamageFromPlayer(soController.playerAttackDamage);
                        //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobClose>().currentHealth);
                    }

                    //DMG du player sur le TrashMobRange
                    if (revealingDashAimedEnemy.CompareTag("TrashMobRange"))
                    {
                        revealingDashAimedEnemy.GetComponent<TrashMobRange>().TakeDamageFromPlayer(soController.playerAttackDamage);
                        Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobRange>().currentHealth);
                    }
            
                    //DMG du player sur le Bully
                    if (revealingDashAimedEnemy.CompareTag("Bully"))
                    {
                        revealingDashAimedEnemy.GetComponent<Bully>().TakeDamageFromPlayer(soController.playerAttackDamage);
                        //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobRange>().currentHealth);
                    }
            
                    //DMG du player sur le caretaker
                    if (revealingDashAimedEnemy.CompareTag("Caretaker"))
                    {
                        revealingDashAimedEnemy.GetComponent<CareTaker>().TakeDamageFromPlayer(soController.playerAttackDamage);
                        //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobRange>().currentHealth);
                    }
                
                    isHitting = false;
                }
            }
        }

        public IEnumerator StunEnemy(GameObject enemy)
        {
            if (enemy.CompareTag("TrashMobClose"))
            {
                enemy.GetComponent<TrashMobClose>().isStunned = true;
                yield return new WaitForSeconds(stunDuration);
                enemy.GetComponent<TrashMobClose>().isStunned = false;
            }

            if (enemy.CompareTag("TrashMobRange"))
            {
                enemy.GetComponent<TrashMobRange>().isStunned = true;
                yield return new WaitForSeconds(stunDuration);
                enemy.GetComponent<TrashMobRange>().isStunned = false;
            }
        
            if (enemy.CompareTag("Bully"))
            {
                enemy.GetComponent<Bully>().isStunned = true;
                yield return new WaitForSeconds(stunDuration);
                enemy.GetComponent<Bully>().isStunned = false;
            }
            
            if (enemy.CompareTag("Caretaker"))
            {
                enemy.GetComponent<CareTaker>().isStunned = true;
                yield return new WaitForSeconds(stunDuration);
                enemy.GetComponent<CareTaker>().isStunned = false;
            }
        }

        #endregion
    
  
    }
}
