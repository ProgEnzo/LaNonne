using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Core.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controller
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField] public Rigidbody2D m_rigidbody;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;
    
        [SerializeField] public float m_timerDash;

        public static PlayerController instance;

        public RoomFirstDungeonGenerator rfg;
        
        public DijkstraAlgorithm dijkstraAlgorithm;
        
        [SerializeField] private List<int> layersToConsiderAnyway = new();
        private List<int> layersToUndoIgnore = new();

        [Header("Revealing Dash")]
        [NonSerialized] public bool isHitting;
        [SerializeField] public float hitSpeed = 1f;
        [SerializeField] public float revealingDashDetectionRadius = 1f;
        [SerializeField] public int revealingDashEpCost;
        [NonSerialized] public GameObject revealingDashAimedEnemy;
        [SerializeField] public float toleranceDistance = 0.1f;
        [NonSerialized] public Vector3 newPosition;
        [SerializeField] public float stunDuration = 1f;
        [NonSerialized] Dictionary<GameObject, Coroutine> runningCoroutines = new();
        [SerializeField] public float damageMultiplier = 1f;
        [SerializeField] public float revealingDashTimer = 5f;
        [NonSerialized] public float revealingDashTimerCount;

        [Header("UI elements")]

        [SerializeField] public Slider hpSlider;
        [SerializeField] public int currentEp;

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
            hpSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            hpSlider.maxValue = soController.maxHealth;
            hpSlider.value = soController.maxHealth;
            currentEp = 0;

            //ReInit();
        }
        
        public void ResetVelocity()
        {
            m_rigidbody.velocity = Vector2.zero;
        }

        /*public void ReInit()
        {
            transform.position = dijkstraAlgorithm.startingPoint.transform.position;
        }*/
    
        void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && m_timerDash < -0.5f)
            {
                for (var i = 0; i < 32; i++)
                {
                    if (Physics2D.GetIgnoreLayerCollision(7, i)) continue;
                    Physics2D.IgnoreLayerCollision(7, i);
                    layersToUndoIgnore.Add(i);
                }
                foreach (var layer in layersToConsiderAnyway)
                {
                    Physics2D.IgnoreLayerCollision(7, layer, false);
                }
                m_timerDash = soController.m_durationDash;
            }
            
            if (m_timerDash < -0.5f)
            {
                foreach (var layer in layersToUndoIgnore)
                {
                    Physics2D.IgnoreLayerCollision(7, layer, false);
                }
                layersToUndoIgnore.Clear();
            }
            else
            {
                m_timerDash -= Time.deltaTime;
            }
        
            RevealingDash();
        }
        public void FixedUpdate()
        {
            m_rigidbody.drag = soController.dragDeceleration * soController.dragMultiplier;
            ManageMove();
        }
        
        public void AddEP(int epGain)
        {
            currentEp += epGain;
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
            hpSlider.value -= damage;
            Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, HEALTH REMAINING : " + soController.currentHealth);

            if (soController.currentHealth <= 0)
            {
                Die();
            }
        }

        private static void Die()
        {
            //Destroy(gameObject);
            Debug.Log("<color=green>PLAYER</color> IS NOW DEAD");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    

        #endregion

        #region AttackPlayer
        void RevealingDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isHitting && soController.epAmount >= revealingDashEpCost && revealingDashTimerCount <= 0)
            {
                List<RaycastHit2D> enemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, revealingDashDetectionRadius, Vector2.zero, new ContactFilter2D(), enemiesInArea);
                enemiesInArea.Sort((x, y) =>
                {
                    Vector3 position = transform.position;
                    return (Vector3.Distance(position, x.transform.position).CompareTo(Vector3.Distance(position, y.transform.position)));
                });
                foreach (RaycastHit2D enemy in enemiesInArea)
                {
                    if (enemy.collider.CompareTag("Enemy"))
                    {
                        soController.epAmount -= revealingDashEpCost;
                        revealingDashAimedEnemy = enemy.collider.gameObject;
                        newPosition = revealingDashAimedEnemy.transform.position;
                        isHitting = true;
                        revealingDashTimerCount = revealingDashTimer;
                        break;
                    }
                }
            }
            else if (revealingDashTimerCount > 0)
            {
                revealingDashTimerCount -= Time.deltaTime;
            }

            if (isHitting)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPosition, hitSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, newPosition) < toleranceDistance)
                {
                    foreach (GameObject enemy in runningCoroutines.Keys)
                    {
                        if (enemy == revealingDashAimedEnemy)
                        {
                            StopCoroutine(runningCoroutines[enemy]);
                            runningCoroutines.Remove(enemy);
                            break;
                        }
                    }
                    runningCoroutines.Add(revealingDashAimedEnemy, StartCoroutine(StunEnemy(revealingDashAimedEnemy)));
                
                    //DMG du player sur le TrashMobClose
                    if (revealingDashAimedEnemy.CompareTag("Enemy"))
                    {
                        revealingDashAimedEnemy.GetComponent<EnemyController>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                        //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobClose>().currentHealth);
                    }

                    isHitting = false;
                }
            }
        }

        public IEnumerator StunEnemy(GameObject enemy)
        {
            enemy.GetComponent<EnemyController>().isStunned = true;
            yield return new WaitForSeconds(stunDuration);
            if (enemy) yield break;
            enemy.GetComponent<EnemyController>().isStunned = false;
        }
        #endregion
    }
}
