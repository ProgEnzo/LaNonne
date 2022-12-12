using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Core.Scripts.Utils;
using DG.Tweening;
using Manager;
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
        
        [Header("Movement")]
        internal (Vector2 horizontal, Vector2 vertical) direction = (new Vector2(0, 0), new Vector2(0, 0));
        
        [Header("Dash")]
        internal float timerDash;
        
        [Header("Health")]
        private int currentHealth;

        [Header("Revealing Dash")]
        internal bool isRevealingDashHitting;
        private GameObject revealingDashAimedEnemy;
        private Vector3 revealingDashNewPosition;
        private readonly Dictionary<GameObject, Coroutine> revealingDashRunningStunCoroutines = new();
        private float revealingDashTotalDistance;
        internal bool isRevealingDashOn;
        private bool isRevealingDashFocusOn;
        private float currentRevealingDashFocusCooldown;
        private float currentRevealingDashCooldown;
        internal float currentSlowMoPlayerMoveSpeedFactor;
        internal float currentSlowMoPlayerAttackSpeedFactor;

        [Header("UI elements")]
        private Image healthBar;
        private Image revealingDashCooldownBar;
        internal int currentEp;
        private UIManager uiManager;
        
        [Header("Animations")]
        private AnimationManager animationManager;
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int AttackState = Animator.StringToHash("attackState");
        private static readonly int SlowMoMoveSpeed = Animator.StringToHash("slowMoMoveSpeed");
        private static readonly int SlowMoAttackSpeed = Animator.StringToHash("slowMoAttackSpeed");
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
            isRevealingDashOn = false;
            healthBar = GameObject.Find("HealthBar").transform.GetChild(0).GetComponent<Image>();
            healthBar.fillAmount = 1f;
            revealingDashCooldownBar = GameObject.Find("RevealingDashCooldown").GetComponent<Image>();
            revealingDashCooldownBar.fillAmount = 1f;
            currentEp = 0;
            currentSlowMoPlayerMoveSpeedFactor = 1f;
            currentSlowMoPlayerAttackSpeedFactor = 1f;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Update()
        {
            if (Input.GetKeyDown(inputManager.dashKey) && timerDash < -0.5f && !isRevealingDashOn)
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
            
            // SlowMoManager();
            RevealingDashStart();
            RevealingDash();
            RevealingDashFocus();
            RevealingDashStop();

            currentAnimPrefabAnimator.SetFloat(SlowMoMoveSpeed, currentSlowMoPlayerMoveSpeedFactor);
            currentAnimPrefabAnimator.SetFloat(SlowMoAttackSpeed, currentSlowMoPlayerAttackSpeedFactor);
            
            if (currentRevealingDashCooldown > 0)
            {
                currentRevealingDashCooldown -= Time.deltaTime;
            }
            revealingDashCooldownBar.fillAmount = 1 - currentRevealingDashCooldown / soController.revealingDashCooldown;
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
            var speed = timerDash <= 0 ? soController.moveSpeed : soController.dashSpeed; // for movement
            var movingState = timerDash <= 0 ? 2 : 3; // for animation

            var nbInputs = (Input.GetKey(inputManager.upMoveKey) ? 1 : 0) + (Input.GetKey(inputManager.leftMoveKey) ? 1 : 0) +
                           (Input.GetKey(inputManager.downMoveKey) ? 1 : 0) + (Input.GetKey(inputManager.rightMoveKey) ? 1 : 0); // for movement
            
            if (nbInputs > 1) speed *= 0.75f; // for movement
            
            var localScale = transform.localScale; // for animation

            direction = (Vector2.zero, Vector2.zero);

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
                mRigidbody.AddForce(Vector2.left * (speed * currentSlowMoPlayerMoveSpeedFactor) / Time.timeScale); // for movement
                direction.horizontal = Vector2.left;
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
                mRigidbody.AddForce(Vector2.right * (speed * currentSlowMoPlayerMoveSpeedFactor) / Time.timeScale); // for movement
                direction.horizontal = direction.horizontal == Vector2.left ? Vector2.zero : Vector2.right;
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
                mRigidbody.AddForce(Vector2.up * (speed * currentSlowMoPlayerMoveSpeedFactor) / Time.timeScale); // for movement
                direction.vertical = Vector2.up;
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
                mRigidbody.AddForce(Vector2.down * (speed * currentSlowMoPlayerMoveSpeedFactor) / Time.timeScale); // for movement
                direction.vertical = direction.vertical == Vector2.up ? Vector2.zero : Vector2.down;
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

        internal static void Die()
        {
            Debug.Log("<color=green>PLAYER</color> IS NOW DEAD");
            UIManager.instance.GameOver();
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

        #region Ex-SlowMo

        /*private void SlowMoManager()
        {
            if (Input.GetKeyDown(inputManager.slowMoKey) && currentSlowMoCooldown <= 0)
            {
                isSlowMoOn = true;
                currentSlowMoDuration = soController.slowMoDuration;
                currentSlowMoPlayerMoveSpeedFactor = soController.slowMoPlayerSpeedFactor;
                currentSlowMoPlayerAttackSpeedFactor = soController.slowMoPlayerSpeedFactor;
                Time.timeScale = 1 / soController.slowMoSpeed;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                if (slowMoSequence == null)
                {
                    slowMoSequence = DOTween.Sequence();
                    slowMoSequence
                        .Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, currentSlowMoDuration)
                            .SetEase(Ease.InQuad)).Append(DOTween.To(() => Time.fixedDeltaTime,
                            x => Time.fixedDeltaTime = x, 0.02f, currentSlowMoDuration).SetEase(Ease.InQuad))
                        .Append(DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x,
                            1, currentSlowMoDuration).SetEase(Ease.InQuad)).Append(DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, 
                            x => currentSlowMoPlayerAttackSpeedFactor = x, 1, currentSlowMoDuration).SetEase(Ease.InQuad));
                    slowMoUid = Guid.NewGuid();
                    slowMoSequence.id = slowMoUid;
                }
            }
            if (Input.GetKey(inputManager.slowMoKey) && currentSlowMoCooldown <= 0 && isSlowMoOn)
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
                    currentSlowMoPlayerMoveSpeedFactor = 1f;
                    currentSlowMoPlayerAttackSpeedFactor = 1f;
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f;
                }
            }
            if (Input.GetKeyUp(inputManager.slowMoKey) && currentSlowMoCooldown <= 0 && isSlowMoOn)
            {
                isSlowMoOn = false;
                currentSlowMoCooldown = soController.slowMoCooldown;
                currentSlowMoPlayerMoveSpeedFactor = 1f;
                DOTween.Kill(slowMoUid);
                slowMoSequence = null;
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.01f);
                DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.02f, 0.01f);
                DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x, 1, 0.01f);
                DOTween.To(() => currentSlowMoPlayerAttackSpeedFactor, x => currentSlowMoPlayerAttackSpeedFactor = x, 1, 0.01f);
            }
            
            if (currentSlowMoCooldown > 0)
            {
                currentSlowMoCooldown -= Time.deltaTime;
            }
        }*/
        
        #endregion
        
        #region RevealingDash
        
        private void RevealingDashStart()
        {
            if (Input.GetKeyDown(inputManager.revealingDashKey) && !isRevealingDashHitting)
            {
                isRevealingDashOn = true;
                
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
                    revealingDashTotalDistance = Vector3.Distance(transform.position, revealingDashNewPosition);
                    
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.1f, revealingDashTotalDistance / soController.revealingDashHitSpeed);
                    DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.002f, revealingDashTotalDistance / soController.revealingDashHitSpeed);
                    return;
                }
            }
        }

        private void RevealingDash()
        {
            if (isRevealingDashHitting)
            {
                //Variables pour code optimisé
                var position = transform.position;
                
                //Dash
                transform.position = Vector2.MoveTowards(position, revealingDashNewPosition, soController.revealingDashHitSpeed * Time.deltaTime);

                //Tant que l'ennemi n'est pas atteint, on ne passe pas à la suite
                if (!(Vector3.Distance(transform.position, revealingDashNewPosition) < soController.revealingDashToleranceDistance)) return;
                
                //Gestion du stun
                foreach (var enemy in revealingDashRunningStunCoroutines.Keys.Where(enemy => enemy == revealingDashAimedEnemy))
                {
                    StopCoroutine(revealingDashRunningStunCoroutines[enemy]);
                    revealingDashRunningStunCoroutines.Remove(enemy);
                    break;
                }
                revealingDashRunningStunCoroutines.Add(revealingDashAimedEnemy, StartCoroutine(StunEnemy(revealingDashAimedEnemy)));
                
                //Dégâts
                if (revealingDashAimedEnemy.CompareTag("Enemy"))
                {
                    revealingDashAimedEnemy.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.revealingDashDamage);
                }
                
                //Fin du dash
                isRevealingDashHitting = false;
                
                //Gestion du slow mo
                DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x, 0.1f, 0.1f).SetEase(Ease.InQuad);
                currentRevealingDashFocusCooldown = soController.revealingDashFocusDuration;
                isRevealingDashFocusOn = true;
            }
        }
        
        private void RevealingDashFocus()
        {
            if (!isRevealingDashFocusOn) return;
            if (currentRevealingDashFocusCooldown > 0)
            {
                if (revealingDashAimedEnemy == null)
                {
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.01f);
                    DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.02f, 0.01f);
                    DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x, 1, 0.01f);
                
                    isRevealingDashFocusOn = false;
                    isRevealingDashOn = false;
                    currentRevealingDashCooldown = soController.revealingDashCooldown;
                }
                currentRevealingDashFocusCooldown -= Time.deltaTime;
            }
            else
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.01f);
                DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.02f, 0.01f);
                DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x, 1, 0.01f);
                
                isRevealingDashFocusOn = false;
                isRevealingDashOn = false;
                currentRevealingDashCooldown = soController.revealingDashCooldown;
            }
        }
        
        private void RevealingDashStop()
        {
            if (Input.GetKeyDown(inputManager.revealingDashKey) && isRevealingDashFocusOn)
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.01f);
                DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, 0.02f, 0.01f);
                DOTween.To(() => currentSlowMoPlayerMoveSpeedFactor, x => currentSlowMoPlayerMoveSpeedFactor = x, 1, 0.01f);
                
                isRevealingDashFocusOn = false;
                isRevealingDashOn = false;
                currentRevealingDashCooldown = soController.revealingDashCooldown;
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
