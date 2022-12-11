using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Core.Scripts.Utils;
using DG.Tweening;
using Manager;
using Pathfinding.Util;
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
        
        [Header("Inputs")]
        private InputManager inputManager;
        
        [Header("Dash")]
        internal float timerDash;
        
        [Header("Health")]
        private int currentHealth;

        [Header("Slow Motion")]
        internal bool isSlowMoOn;
        private float currentSlowMoCooldown;
        private float currentSlowMoDuration;
        internal float currentSlowMoPlayerSpeedFactor;
        internal Sequence slowMoSequence;
        internal Guid slowMoUid;

        [Header("Revealing Dash")]
        private bool isRevealingDashHitting;
        private GameObject revealingDashAimedEnemy;
        private Vector3 revealingDashNewPosition;
        private readonly Dictionary<GameObject, Coroutine> revealingDashRunningStunCoroutines = new();

        [Header("UI elements")]
        private Image healthBar;
        private Image chrono;
        internal int currentEp;
        
        [Header("Animations")]
        private AnimationManager animationManager;
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int AttackState = Animator.StringToHash("attackState");
        private static readonly int SlowMoSpeed = Animator.StringToHash("slowMoSpeed");
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


        // public Vector2 bossPos;
        // public Vector2 currentPos;

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
            inputManager = InputManager.instance;
            playerScale = transform.localScale.x;
            currentHealth = soController.maxHealth;
            isRevealingDashHitting = false;
            healthBar = GameObject.Find("HealthBar").transform.GetChild(0).GetComponent<Image>();
            healthBar.fillAmount = 1f;
            chrono = GameObject.Find("Chrono").GetComponent<Image>();
            chrono.fillAmount = 1f;
            currentEp = 0;
            currentSlowMoPlayerSpeedFactor = 1f;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(inputManager.dashKey) && timerDash < -0.5f && !isSlowMoOn)
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
            
            SlowMoManager();
            RevealingDashStart();
            LoadMenu();
            currentAnimPrefabAnimator.SetFloat(SlowMoSpeed, currentSlowMoPlayerSpeedFactor);
            chrono.fillAmount = 1 - currentSlowMoCooldown / soController.slowMoCooldown;
        }
        
        public void FixedUpdate()
        {
            mRigidbody.drag = soController.dragDeceleration * soController.dragMultiplier;
            ManageMove();
            RevealingDash();
        }
        
        public void AddEp(int epGain)
        {
            currentEp += epGain;
        }
    
        #region MovementPlayer
        
        private void ManageMove()
        {
            var speed = timerDash <= 0 ? soController.moveSpeed : soController.dashSpeed; // for movement
            var movingState = timerDash <= 0 ? 2 : 3; // for animation

            var nbInputs = (Input.GetKey(inputManager.upMoveKey) ? 1 : 0) + (Input.GetKey(inputManager.leftMoveKey) ? 1 : 0) +
                           (Input.GetKey(inputManager.downMoveKey) ? 1 : 0) + (Input.GetKey(inputManager.rightMoveKey) ? 1 : 0); // for movement
            
            if (nbInputs > 1) speed *= 0.75f; // for movement
            
            var localScale = transform.localScale; // for animation

            if (Input.GetKey(inputManager.leftMoveKey)) // for input
            {
                isMoving = true; // for animation
                isMovingProfile = true; // for movement
                transform.localScale = new Vector3(-playerScale, localScale.y, localScale.z); // for animation
                transform.GetChild(0).localScale = new Vector3(1, -1, 1); // for animation
                if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 2) // for animation
                {
                    animParametersToChange = (DirectionState, 2); // for animation
                }
                mRigidbody.AddForce(Vector2.left * (speed * currentSlowMoPlayerSpeedFactor) / Time.timeScale); // for movement
            }

            if (Input.GetKey(inputManager.rightMoveKey)) // for input
            {
                isMoving = true; // for animation
                isMovingProfile = true; // for movement
                transform.localScale = new Vector3(playerScale, localScale.y, localScale.z); // for animation
                transform.GetChild(0).localScale = new Vector3(1, 1, 1); // for animation
                if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 2) // for animation
                {
                    animParametersToChange = (DirectionState, 2); // for animation
                }
                mRigidbody.AddForce(Vector2.right * (speed * currentSlowMoPlayerSpeedFactor) / Time.timeScale); // for movement
            }

            if (Input.GetKey(inputManager.upMoveKey)) // for input
            {
                isMoving = true; // for animation
                if (!isMovingProfile) // for movement
                {
                    transform.localScale = new Vector3(playerScale, localScale.y, localScale.z); // for animation
                    transform.GetChild(0).localScale = new Vector3(1, 1, 1); // for animation
                    if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 1) // for animation
                    {
                        animParametersToChange = (DirectionState, 1); // for animation
                    }
                }
                mRigidbody.AddForce(Vector2.up * (speed * currentSlowMoPlayerSpeedFactor) / Time.timeScale); // for movement
            }

            if (Input.GetKey(inputManager.downMoveKey)) // for input
            {
                isMoving = true; // for animation
                if (!isMovingProfile) // for movement
                {
                    transform.localScale = new Vector3(playerScale, localScale.y, localScale.z); // for animation
                    transform.GetChild(0).localScale = new Vector3(1, 1, 1); // for animation
                    if (currentAnimPrefabAnimator.GetInteger(DirectionState) != 0) // for animation
                    {
                        animParametersToChange = (DirectionState, 0); // for movement
                    }
                }
                mRigidbody.AddForce(Vector2.down * (speed * currentSlowMoPlayerSpeedFactor) / Time.timeScale); // for movement
            }

            if (!isRevealingDashHitting)
            {
                if (currentAnimPrefabAnimator.GetInteger(AttackState) == 0) // all for animation
                {
                    if (isMoving)
                    {
                        if (currentAnimPrefabAnimator.GetInteger(MovingState) != movingState)
                        {
                            animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, movingState);
                        }
                        else
                        {
                            if (animParametersToChange != (0, 0))
                            {
                                animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange.parameterToChange, animParametersToChange.value);
                            }
                        }
                    }
                    else
                    {
                        if (currentAnimPrefabAnimator.GetInteger(MovingState) != 1)
                        {
                            animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 1);
                        }
                        else
                        {
                            if (animParametersToChange != (0, 0))
                            {
                                animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange.parameterToChange, animParametersToChange.value);
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentAnimPrefabAnimator.GetInteger(MovingState) != 3)
                {
                    animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange == (0, 0) ? MovingState : animParametersToChange.value, 3);
                }
                else
                {
                    if (animParametersToChange != (0, 0))
                    {
                        animationManager.AnimationControllerPlayer(animPrefabs, ref currentAnimPrefab, ref currentAnimPrefabAnimator, animParametersToChange.parameterToChange, 3);
                    }
                }
            }
            isMoving = false; // for animation
            isMovingProfile = false; // for movement
            animParametersToChange = (0, 0); // for animation
        }
        
        #endregion

        #region HealthPlayer
        
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.fillAmount = (float)currentHealth / soController.maxHealth;
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
            healthBar.fillAmount = (float)currentHealth / soController.maxHealth;
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

        private void SlowMoManager()
        {
            if (Input.GetKeyDown(inputManager.slowMoKey) && currentSlowMoCooldown <= 0)
            {
                isSlowMoOn = true;
                currentSlowMoDuration = soController.slowMoDuration;
                currentSlowMoPlayerSpeedFactor = soController.slowMoPlayerSpeedFactor;
                Time.timeScale = 1 / soController.slowMoSpeed;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                if (slowMoSequence == null)
                {
                    slowMoSequence = DOTween.Sequence();
                    slowMoSequence
                        .Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, currentSlowMoDuration)
                            .SetEase(Ease.InQuad)).Append(DOTween.To(() => Time.fixedDeltaTime,
                            x => Time.fixedDeltaTime = x, 0.02f, currentSlowMoDuration).SetEase(Ease.InQuad))
                        .Append(DOTween.To(() => currentSlowMoPlayerSpeedFactor, x => currentSlowMoPlayerSpeedFactor = x,
                            1, currentSlowMoDuration).SetEase(Ease.InQuad));
                    slowMoUid = Guid.NewGuid();
                    slowMoSequence.id = slowMoUid;
                }
            }
            if (Input.GetKey(inputManager.slowMoKey) && currentSlowMoCooldown <= 0)
            {
                if (currentSlowMoDuration > 0)
                {
                    currentSlowMoDuration -= Time.deltaTime;
                }
                else
                {
                    isSlowMoOn = false;
                    currentSlowMoCooldown = soController.slowMoCooldown;
                    DOTween.Kill(slowMoUid);
                    slowMoSequence = null;
                    currentSlowMoPlayerSpeedFactor = 1f;
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f;
                }
            }
            if (Input.GetKeyUp(inputManager.slowMoKey) && currentSlowMoCooldown <= 0 && isSlowMoOn)
            {
                isSlowMoOn = false;
                currentSlowMoCooldown = soController.slowMoCooldown;
                currentSlowMoPlayerSpeedFactor = 1f;
                DOTween.Kill(slowMoUid);
                slowMoSequence = null;
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.01f);
                DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.02f, 0.01f);
                DOTween.To(() => currentSlowMoPlayerSpeedFactor, x => currentSlowMoPlayerSpeedFactor = x, 1, 0.01f);
            }
            
            if (currentSlowMoCooldown > 0)
            {
                currentSlowMoCooldown -= Time.deltaTime;
            }
        }

        private void RevealingDash()
        {
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
                    revealingDashAimedEnemy.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.revealingDashDamage);
                    //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + revealingDashAimedEnemy.GetComponent<TrashMobClose>().currentHealth);
                }

                isRevealingDashHitting = false;
            }
        }

        private void RevealingDashStart()
        {
            if (Input.GetKeyDown(inputManager.revealingDashKey) && !isRevealingDashHitting && isSlowMoOn)
            {
                var enemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, soController.revealingDashDetectionRadius, Vector2.zero,
                    new ContactFilter2D(), enemiesInArea);
                enemiesInArea.Sort((x, y) =>
                {
                    var position = transform.position;
                    return (Vector3.Distance(position, x.transform.position)
                        .CompareTo(Vector3.Distance(position, y.transform.position)));
                });
                foreach (var enemy in enemiesInArea.Where(enemy => enemy.collider.CompareTag("Enemy")))
                {
                    revealingDashAimedEnemy = enemy.collider.gameObject;
                    revealingDashNewPosition = revealingDashAimedEnemy.transform.position;
                    isRevealingDashHitting = true;
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1/soController.revealingDashSlowTimeSpeed, 0.1f);
                    break;
                }
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
