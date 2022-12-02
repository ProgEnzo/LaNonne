using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Core.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controller
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        [SerializeField] public Rigidbody2D mRigidbody;
        private CapsuleCollider2D collider2d;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
    
        [SerializeField] public float mTimerDash;

        public new static PlayerController instance;

        [Header("Revealing Dash")]
        private bool isHitting;
        [SerializeField] public float hitSpeed = 1f;
        [SerializeField] public float revealingDashDetectionRadius = 1f;
        private GameObject revealingDashAimedEnemy;
        [SerializeField] public float toleranceDistance = 0.1f;
        private Vector3 newPosition;
        [SerializeField] public float stunDuration = 1f;
        private readonly Dictionary<GameObject, Coroutine> runningCoroutines = new();
        [SerializeField] public float damageMultiplier = 1f;
        [SerializeField] public float revealingDashTimer = 5f;
        private float revealingDashTimerCount;

        [Header("UI elements")]

        [SerializeField] public Slider hpSlider;
        [SerializeField] public int currentEp;
        private static readonly int CanChange = Animator.StringToHash("canChange");
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
        private int currentHealth;

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
            playerScale = transform.localScale.x;
            currentHealth = soController.maxHealth;
            isHitting = false;
            hpSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            hpSlider.maxValue = soController.maxHealth;
            hpSlider.value = soController.maxHealth;
            currentEp = 0;

            //ReInit();
        }
        
        /*public void ResetVelocity()
        {
            mRigidbody.velocity = Vector2.zero;
        }*/

        /*public void ReInit()
        {
            transform.position = dijkstraAlgorithm.startingPoint.transform.position;
        }*/

        private void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && mTimerDash < -0.5f)
            {
                collider2d.enabled = false;
                mTimerDash = soController.durationDash;
            }
            
            if (mTimerDash < -0.5f)
            {
                collider2d.enabled = true;
            }
            else
            {
                mTimerDash -= Time.deltaTime;
            }
        
            RevealingDash();
            LoadMenu();
        }
        public void FixedUpdate()
        {
            mRigidbody.drag = soController.dragDeceleration * soController.dragMultiplier;
            ManageMove();
        }
        
        public void AddEP(int epGain)
        {
            currentEp += epGain;
        }
    
        #region MovementPlayer
        private void ManageMove()
        {
            var speed = mTimerDash <= 0 ? soController.moveSpeed : soController.dashSpeed;

            int nbInputs = (Input.GetKey(KeyCode.Z) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? 1 : 0) +
                           (Input.GetKey(KeyCode.S) ? 1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            
            if (nbInputs > 1) speed *= 0.75f;

            if (Input.GetKey(KeyCode.Q))
            {
                isMoving = true;
                isMovingProfile = true;
                transform.localScale = new Vector3(-playerScale, transform.localScale.y, transform.localScale.z);
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
                transform.localScale = new Vector3(playerScale, transform.localScale.y, transform.localScale.z);
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
                    transform.localScale = new Vector3(playerScale, transform.localScale.y, transform.localScale.z);
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
                    transform.localScale = new Vector3(playerScale, transform.localScale.y, transform.localScale.z);
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
                        AnimationControllerInt(
                            animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 1);
                    }
                    else
                    {
                        if (animParametersToChange != (0, 0))
                        {
                            AnimationControllerInt(animParametersToChange.parameterToChange, animParametersToChange.value);
                        }
                    }
                }
                else
                {
                    if (currentAnimPrefabAnimator.GetInteger(MovingState) != 0)
                    {
                        AnimationControllerInt(
                            animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 0);
                    }
                    else
                    {
                        if (animParametersToChange != (0, 0))
                        {
                            AnimationControllerInt(animParametersToChange.parameterToChange, animParametersToChange.value);
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
        
        public void LoadMenu()
        {
            if (currentHealth <= 0)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        internal static void Die()
        {
            //Destroy(gameObject);
            Debug.Log("<color=green>PLAYER</color> IS NOW DEAD");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        #endregion

        #region AttackPlayer

        private void RevealingDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isHitting && revealingDashTimerCount <= 0)
            {
                var enemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, revealingDashDetectionRadius, Vector2.zero, new ContactFilter2D(), enemiesInArea);
                enemiesInArea.Sort((x, y) =>
                {
                    var position = transform.position;
                    return (Vector3.Distance(position, x.transform.position).CompareTo(Vector3.Distance(position, y.transform.position)));
                });
                foreach (var enemy in enemiesInArea.Where(enemy => enemy.collider.CompareTag("Enemy")))
                {
                    revealingDashAimedEnemy = enemy.collider.gameObject;
                    newPosition = revealingDashAimedEnemy.transform.position;
                    isHitting = true;
                    revealingDashTimerCount = revealingDashTimer;
                    break;
                }
            }
            else if (revealingDashTimerCount > 0)
            {
                revealingDashTimerCount -= Time.deltaTime;
            }

            if (isHitting)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPosition, hitSpeed * Time.deltaTime);
                if (!(Vector3.Distance(transform.position, newPosition) < toleranceDistance)) return;
                foreach (var enemy in runningCoroutines.Keys.Where(enemy => enemy == revealingDashAimedEnemy))
                {
                    StopCoroutine(runningCoroutines[enemy]);
                    runningCoroutines.Remove(enemy);
                    break;
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

        private IEnumerator StunEnemy(GameObject enemy)
        {
            enemy.GetComponent<EnemyController>().isStunned = true;
            yield return new WaitForSeconds(stunDuration);
            Debug.Log(enemy);
            if (!enemy) yield break;
            enemy.GetComponent<EnemyController>().isStunned = false;
        }
        #endregion
        
        private void AnimationControllerInt(int parameterToChange, int value)
        {
            if (parameterToChange  == DirectionState || parameterToChange == MovingState)
            {
                if (currentAnimPrefabAnimator.GetInteger(parameterToChange) != value)
                {
                    AnimationManagerInt(parameterToChange, value);
                    StartCoroutine(CanChangeCoroutine());
                }
            }
            else
            {
                if (!(currentAnimPrefabAnimator.GetInteger(DirectionState) == parameterToChange &&
                     currentAnimPrefabAnimator.GetInteger(MovingState) == value))
                {
                    AnimationManagerInt(parameterToChange, value);
                    StartCoroutine(CanChangeCoroutine());
                }
            }
        }

        internal IEnumerator CanChangeCoroutine()
        {
            currentAnimPrefabAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(CanChange, false);
        }

        /*private IEnumerator AnimationControllerBool(int parameterToChange)
        {
            currentAnimPrefabAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(CanChange, false);
            currentAnimPrefabAnimator.SetBool(parameterToChange, true);
            yield return new WaitForNextFrameUnit();
            currentAnimPrefabAnimator.SetBool(parameterToChange, false);
        }*/

        private void AnimationManagerInt(int parameterToChange, int value)
        {
            if (parameterToChange  == DirectionState)
            {
                AnimationManagerSwitch(value, currentAnimPrefabAnimator.GetInteger(MovingState),
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }
            else if (parameterToChange == MovingState)
            {
                AnimationManagerSwitch(currentAnimPrefabAnimator.GetInteger(DirectionState), value,
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }
            else
            {
                AnimationManagerSwitch(parameterToChange, value, 
                    currentAnimPrefabAnimator.GetBool(IsAttacking));
            }

            foreach (var prefab in animPrefabs.Where(prefab => prefab != currentAnimPrefab))
            {
                prefab.SetActive(false);
            }
        }

        internal void AnimationManagerSwitch(int directionState, int movingState, bool isAttacking)
        {
            currentAnimPrefab = (directionState, movingState, isAttacking) switch
            {
                (0, 0, false) => animPrefabs[0],
                (0, 1, false) => animPrefabs[1],
                (0, >= 0, true) => animPrefabs[2],
                (1, 0, false) => animPrefabs[3],
                (1, 1, false) => animPrefabs[4],
                (1, >= 0, true) => animPrefabs[5],
                (2, 0, false) => animPrefabs[6],
                (2, 1, false) => animPrefabs[7],
                (2, >= 0, true) => animPrefabs[8],
                _ => animPrefabs[0]
            };

            currentAnimPrefab.SetActive(true);
            currentAnimPrefabAnimator = currentAnimPrefab.GetComponent<Animator>();
            
            currentAnimPrefabAnimator.SetInteger(DirectionState, directionState);
            currentAnimPrefabAnimator.SetInteger(MovingState, movingState);
            currentAnimPrefabAnimator.SetBool(IsAttacking, isAttacking);
        }
    }
}
