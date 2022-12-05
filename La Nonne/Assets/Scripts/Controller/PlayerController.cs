using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.Boss;
using Core.Scripts.Utils;
using DG.Tweening;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace Controller
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [Header("Instance")]
        internal new static PlayerController instance;
        
        [Header("Components")]
        internal Rigidbody2D mRigidbody;
        private CapsuleCollider2D collider2d;
        private CapsuleCollider2D childrenCollider2d;
    
        [Header("Scriptable Object")]
        [SerializeField][FormerlySerializedAs("SO_Controller")] internal SO_Controller soController;
        
        [Header("Dash")]
        internal float timerDash;
        
        [Header("Health")]
        private int currentHealth;

        [Header("Revealing Dash")]
        private bool isRevealingDashHitting;
        private GameObject revealingDashAimedEnemy;
        private Vector3 revealingDashNewPosition;
        private readonly Dictionary<GameObject, Coroutine> revealingDashRunningStunCoroutines = new();
        private float revealingDashTimerCount;

        [Header("UI elements")]
        [SerializeField] private Slider hpSlider;
        internal int currentEp;
        
        [Header("Animations")]
        private AnimationManager animationManager;
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private bool isMoving;
        private float playerScale;
        internal readonly List<GameObject> animPrefabs = new();
        internal GameObject currentAnimPrefab;
        internal Animator currentAnimPrefabAnimator;
        private (int parameterToChange, int value) animParametersToChange;
        private bool isMovingProfile;
        
        [SerializeField, Space, Header("PostProcessing")] //mettre ces variables dans le scripts du player
        private Volume volume;
        private ChromaticAberration chromaticAberration;
        private float minCA = 5f;
        private float maxCA = 15f; //Lerp the value between those 2 en fonction de la distance player/boss, donc dans un update


        public Vector2 bossPos;
        public Vector2 currentPos;

        /*float GetDistanceBetweenBossAndPlayer(Vector2 currentPos)
        {
            float distance = 99999.9999f;

            if (Vector2.Distance(currentPos, bossPos) < distance)
            {
                distance = Vector2.Distance(currentPos, bossPos);
            }

            return distance;
        }*/

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
            mRigidbody = GetComponent<Rigidbody2D>();
            collider2d = GetComponent<CapsuleCollider2D>();
            
            for (var i = 2; i < transform.childCount; i++)
            {
                animPrefabs.Add(transform.GetChild(i).gameObject);
            }
            currentAnimPrefab = animPrefabs[0];
            currentAnimPrefab.SetActive(true);
            currentAnimPrefabAnimator = currentAnimPrefab.GetComponent<Animator>();
            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefab))
            {
                prefab.SetActive(false);
            }
        }

        private void Start()
        {
            animationManager = AnimationManager.instance;
            playerScale = transform.localScale.x;
            currentHealth = soController.maxHealth;
            isRevealingDashHitting = false;
            hpSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            hpSlider.maxValue = soController.maxHealth;
            hpSlider.value = soController.maxHealth;
            currentEp = 0;
            
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && timerDash < -0.5f)
            {
                collider2d.enabled = false;
                timerDash = soController.durationDash;
            }
            
            if (timerDash < -0.5f)
            {
                collider2d.enabled = true;
            }
            else
            {
                timerDash -= Time.deltaTime;
            }
        
            RevealingDash();
            LoadMenu();

            
        }
        public void FixedUpdate()
        {
            mRigidbody.drag = soController.dragDeceleration * soController.dragMultiplier;
            ManageMove();
        }
        
        public void AddEp(int epGain)
        {
            currentEp += epGain;
        }
    
        #region MovementPlayer
        private void ManageMove()
        {
            var speed = timerDash <= 0 ? soController.moveSpeed : soController.dashSpeed;

            int nbInputs = (Input.GetKey(KeyCode.Z) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? 1 : 0) +
                           (Input.GetKey(KeyCode.S) ? 1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            
            if (nbInputs > 1) speed *= 0.75f;
            
            var localScale = transform.localScale;

            if (Input.GetKey(KeyCode.Q))
            {
                isMoving = true;
                isMovingProfile = true;
                transform.localScale = new Vector3(-playerScale, localScale.y, localScale.z);
                transform.GetChild(0).localScale = new Vector3(1, -1, 1);
                if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 2)
                {
                    animParametersToChange = (DirectionState, 2);
                }
                mRigidbody.AddForce(Vector2.left*speed);
            }

            if (Input.GetKey(KeyCode.D))
            {
                isMoving = true;
                isMovingProfile = true;
                transform.localScale = new Vector3(playerScale, localScale.y, localScale.z);
                transform.GetChild(0).localScale = new Vector3(1, 1, 1);
                if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 2)
                {
                    animParametersToChange = (DirectionState, 2);
                }
                mRigidbody.AddForce(Vector2.right*speed);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                isMoving = true;
                if (!isMovingProfile)
                {
                    transform.localScale = new Vector3(playerScale, localScale.y, localScale.z);
                    transform.GetChild(0).localScale = new Vector3(1, 1, 1);
                    if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 1)
                    {
                        animParametersToChange = (DirectionState, 1);
                    }
                }
                mRigidbody.AddForce(Vector2.up*speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                isMoving = true;
                if (!isMovingProfile)
                {
                    transform.localScale = new Vector3(playerScale, localScale.y, localScale.z);
                    transform.GetChild(0).localScale = new Vector3(1, 1, 1);
                    if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 0)
                    {
                        animParametersToChange = (DirectionState, 0);
                    }
                }
                mRigidbody.AddForce(Vector2.down*speed);
            }

            if (!currentAnimPrefabAnimator.GetBool(IsAttacking))
            {
                if (isMoving)
                {
                    if (currentAnimPrefabAnimator.GetInteger(MovingState) != 1)
                    {
                        animationManager.AnimationControllerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator,
                            animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 1);
                    }
                    else
                    {
                        if (animParametersToChange != (0, 0))
                        {
                            animationManager.AnimationControllerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange.parameterToChange, animParametersToChange.value);
                        }
                    }
                }
                else
                {
                    if (currentAnimPrefabAnimator.GetInteger(MovingState) != 0)
                    {
                        animationManager.AnimationControllerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator,
                            animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 0);
                    }
                    else
                    {
                        if (animParametersToChange != (0, 0))
                        {
                            animationManager.AnimationControllerInt(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange.parameterToChange, animParametersToChange.value);
                        }
                    }
                }
            }
            isMoving = false;
            isMovingProfile = false;
            animParametersToChange = (0, 0);
        }
        #endregion

        #region HealthPlayer
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            hpSlider.value -= damage;
            //Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, HEALTH REMAINING : " + soController.currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void LoadMenu()
        {
            if (currentHealth <= 0)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        internal static void Die()
        {
            Debug.Log("<color=green>PLAYER</color> IS NOW DEAD");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        internal void HealPlayer(int heal)
        {
            currentHealth += heal;
            hpSlider.value += heal;
            HealCeiling();
        }

        private void HealCeiling()
        {
            if (currentHealth > soController.maxHealth)
            {
                currentHealth = soController.maxHealth;
            }
        }
        
        #endregion

        #region AttackPlayer

        private void RevealingDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isRevealingDashHitting && revealingDashTimerCount <= 0)
            {
                var enemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, soController.revealingDashDetectionRadius, Vector2.zero, new ContactFilter2D(), enemiesInArea);
                enemiesInArea.Sort((x, y) =>
                {
                    var position = transform.position;
                    return (Vector3.Distance(position, x.transform.position).CompareTo(Vector3.Distance(position, y.transform.position)));
                });
                foreach (var enemy in enemiesInArea.Where(enemy => enemy.collider.CompareTag("Enemy")))
                {
                    revealingDashAimedEnemy = enemy.collider.gameObject;
                    revealingDashNewPosition = revealingDashAimedEnemy.transform.position;
                    isRevealingDashHitting = true;
                    revealingDashTimerCount = soController.revealingDashTimer;
                    //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.1f, 0.1f);
                    break;
                }
            }
            else if (revealingDashTimerCount > 0)
            {
                revealingDashTimerCount -= Time.deltaTime;
            }

            if (isRevealingDashHitting)
            {
                transform.position = Vector2.MoveTowards(transform.position, revealingDashNewPosition, soController.revealingDashHitSpeed * Time.deltaTime);
                if (!(Vector3.Distance(transform.position, revealingDashNewPosition) < soController.revealingDashToleranceDistance)) return;
                foreach (var enemy in revealingDashRunningStunCoroutines.Keys.Where(enemy => enemy == revealingDashAimedEnemy))
                {
                    StopCoroutine(revealingDashRunningStunCoroutines[enemy]);
                    revealingDashRunningStunCoroutines.Remove(enemy);
                    break;
                }
                revealingDashRunningStunCoroutines.Add(revealingDashAimedEnemy, StartCoroutine(StunEnemy(revealingDashAimedEnemy)));
                
                //DMG du player sur le TrashMobClose
                if (revealingDashAimedEnemy.CompareTag("Enemy"))
                {
                    revealingDashAimedEnemy.GetComponent<EnemyController>().TakeDamageFromPlayer((int)(soController.revealingDashDamage));
                    //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobClose>().currentHealth);
                }

                isRevealingDashHitting = false;
            }
        }

        private IEnumerator StunEnemy(GameObject enemy)
        {
            enemy.GetComponent<EnemyController>().isStunned = true;
            yield return new WaitForSeconds(soController.revealingDashStunDuration);
            Debug.Log(enemy);
            if (!enemy) yield break;
            enemy.GetComponent<EnemyController>().isStunned = false;
        }
        #endregion

        /*private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("ModifiedRoomEntry"))
            {
                for (int i = 0; i < i; i++)
                {
                    GetComponentInChildren<CapsuleCollider2D>().enabled = false;
                }
            }
        }*/
    }
}
