using System.Collections.Generic;
using System.Linq;
using AI;
using AI.Boss;
using Manager;
using Shop;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable CommentTypo

namespace Controller
{
    public class Blade : MonoBehaviour
    {
        private bool isHitting;
        private LineRenderer lineRenderer;
        private BoxCollider2D boxCollider;
        private Quaternion finalRotation1;
        private Quaternion finalRotation2;
        private PlayerController playerController;

        [Header("Animations")]
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int AttackState = Animator.StringToHash("attackState");
        
        private int hitState;
        private float currentDuringComboCooldown;
        private float currentNextComboCooldown;
    
        [SerializeField] private SO_Controller soController;
        private EffectManager effectManager;
        private AnimationManager animationManager;
        private InputManager inputManager;
        private ScoreManager scoreManager;
        private CamManager camManager;
        private UIManager uiManager;

        [Header("Sound")] 
        [SerializeField] private AudioSource playerAudioSource;
        [SerializeField] private AudioClip hitBlade;
        [SerializeField] private AudioClip noHitBlade;


        private void Start()
        {
            playerController = PlayerController.instance;
            effectManager = EffectManager.instance;
            animationManager = AnimationManager.instance;
            scoreManager = ScoreManager.instance;
            inputManager = InputManager.instance;
            camManager = CamManager.instance;
            uiManager = UIManager.instance;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
            currentDuringComboCooldown = 0;
            currentNextComboCooldown = 0;
            hitState = 0;

            playerAudioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            currentDuringComboCooldown -= Time.unscaledDeltaTime;
            currentNextComboCooldown -= Time.unscaledDeltaTime;
            if (currentDuringComboCooldown <= 0)
            {
                hitState = 0;
                camManager.DezoomDuringCombo(hitState);
            }

            if (Input.GetKeyDown(inputManager.zealousBladeKey) && !isHitting && currentNextComboCooldown <= 0 && !playerController.isRevealingDashHitting && !uiManager.isGamePaused && !uiManager.isShopOpened && !uiManager.isWhipMenuOpened)
            {
                ZealousBladeStart();
            }
            
            lineRenderer.SetPosition(1, new Vector3(0, soController.zealousBladeHitLength/parentLocalScaleX, 0));
        }

        private void FixedUpdate()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            ZealousBlade();
            
            boxCollider.size = new Vector2(0.1f/Mathf.Abs(parentLocalScaleX), soController.zealousBladeHitLength/Mathf.Abs(parentLocalScaleX));
            boxCollider.offset = new Vector2(0, soController.zealousBladeHitLength/parentLocalScaleX/2);
        }

        private void ZealousBladeStart()
        {
            hitState += 1;
            currentDuringComboCooldown = soController.zealousBladeMaxDuringComboCooldown;
            
            var facingDirection = playerController.direction.horizontal + playerController.direction.vertical;
            if (facingDirection == Vector2.zero)
            {
                facingDirection = (playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), playerController.transform.localScale.x) switch
                {
                    (0, > 0 or < 0) => Vector2.down,
                    (1, > 0 or < 0) => Vector2.up,
                    (2, > 0) => Vector2.right,
                    (2, < 0) => Vector2.left,
                    _ => Vector2.right
                };
            }
            var transform1 = transform;
            transform1.rotation = Quaternion.LookRotation(Vector3.forward, facingDirection);
            GameObject enemyToAim = null;
            var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'attaque
            Physics2D.CircleCast(transform1.position, soController.zealousBladeHitLength, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'attaque
            if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
            {
                foreach (var hit in from hit in objectsInArea where hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss") let enemyToAimPosition = hit.collider.transform.position let playerPosition = playerController.transform.position let directionToAim = enemyToAimPosition - playerPosition let angleToAim = Vector2.Angle(facingDirection, directionToAim) where angleToAim <= soController.zealousBladeMaxDetectionAngle/2 select hit)
                {
                    enemyToAim = hit.collider.gameObject;
                    break; //On sort de la boucle
                }
            }

            Vector3 newDirection;
            if (enemyToAim)
            {
                newDirection = enemyToAim.transform.position - transform1.position;
                newDirection.z = 0;
                newDirection.Normalize();
            }
            else
            {
                newDirection = facingDirection;
            }
            
            var newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
            animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, hitState);
            finalRotation1 = newRotation * Quaternion.Euler(0, 0, soController.zealousBladeHitAngle / 2);
            finalRotation2 = newRotation * Quaternion.Euler(0, 0, -soController.zealousBladeHitAngle / 2);
            lineRenderer.enabled = true;
            boxCollider.enabled = true;
            isHitting = true;

            transform1.rotation = hitState % 2 == 1 ? finalRotation2 : finalRotation1;
        }

        private void ZealousBlade()
        {
            if (isHitting)
            {
                if (hitState % 2 == 1)
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation1, soController.zealousBladeHitSpeed * Time.unscaledDeltaTime * playerController.currentSlowMoPlayerAttackSpeedFactor);
                    if (transform.rotation.eulerAngles.z < finalRotation1.eulerAngles.z + soController.zealousBladeToleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation1.eulerAngles.z - soController.zealousBladeToleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, 0);
                        if (hitState == soController.zealousBladeMaxHitState)
                        {
                            currentNextComboCooldown = soController.zealousBladeMaxNextComboCooldown;
                            hitState = 0;
                            camManager.DezoomDuringCombo(hitState);
                        }
                    }
                }
                else
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation2, soController.zealousBladeHitSpeed * Time.unscaledDeltaTime * playerController.currentSlowMoPlayerAttackSpeedFactor);
                    if (transform.rotation.eulerAngles.z < finalRotation2.eulerAngles.z + soController.zealousBladeToleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation2.eulerAngles.z - soController.zealousBladeToleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, 0);
                        if (hitState == soController.zealousBladeMaxHitState)
                        {
                            currentNextComboCooldown = soController.zealousBladeMaxNextComboCooldown;
                            hitState = 0;
                            camManager.DezoomDuringCombo(hitState);
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var o = other.gameObject;
            
            playerAudioSource.PlayOneShot(noHitBlade);
            
            //DMG du player sur les enemy
            if (o.CompareTag("Enemy"))
            {
                camManager.DezoomDuringCombo(hitState);
                o.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.zealousBladeDamage);
                if (hitState == soController.zealousBladeMaxHitState)
                {
                    o.GetComponent<EnemyController>().HitStopAndKnockBack(soController.zealousBladeBigHitStopDuration, soController.zealousBladeBigKnockBackForce);
                    
                    //ADD SCORE BIG HIT
                    if (playerController.isRevealingDashFocusOn)
                    {
                        scoreManager.AddRevealingDashBladeHitScore(6);
                    }
                    else
                    {
                        scoreManager.AddBladeHitScore(3);
                    }
                }
                else
                {
                    o.GetComponent<EnemyController>().HitStopAndKnockBack(soController.zealousBladeLittleHitStopDuration, soController.zealousBladeLittleKnockBackForce);
                    
                    //ADD SCORE SMALL HIT
                    if (playerController.isRevealingDashFocusOn)
                    {
                        scoreManager.AddRevealingDashBladeHitScore(2);
                    }
                    else
                    {
                        scoreManager.AddBladeHitScore(1);
                    }
                }
                PutStack(o);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }
            
            //DMG du player sur le BOSS
            if (o.CompareTag("Boss"))
            {
                camManager.DezoomDuringCombo(hitState);
                o.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(soController.zealousBladeDamage);
                if (hitState == soController.zealousBladeMaxHitState)
                {
                    o.GetComponent<BossStateManager>().HitStopAndKnockBack(soController.zealousBladeBigHitStopDuration, soController.zealousBladeBigKnockBackForce);
                    
                    //ADD SCORE BIG HIT
                    if (playerController.isRevealingDashFocusOn)
                    {
                        scoreManager.AddBladeHitScore(6);
                    }
                    else
                    {
                        scoreManager.AddBladeHitScore(3);
                    }
                }
                else
                {
                    o.GetComponent<BossStateManager>().HitStopAndKnockBack(soController.zealousBladeLittleHitStopDuration, soController.zealousBladeLittleKnockBackForce);
                    
                    //ADD SCORE SMALL HIT
                    if (playerController.isRevealingDashFocusOn)
                    {
                        scoreManager.AddBladeHitScore(2);
                    }
                    else
                    {
                        scoreManager.AddBladeHitScore(1);
                    }
                }
                PutStack(o);
            }
        }

        private void PutStack(GameObject enemy)
        {
            var applicableEffects = (from effect in effectManager.appliedEffects let randomPercent = Random.Range(0, 100) where effect != EffectManager.Effect.None && randomPercent < effectManager.effectDictionary[(int)effect][effectManager.effectInventory[effect] - 1].chanceToBeApplied select (effect, effectManager.effectInventory[effect])).ToList();
            if (applicableEffects.Count <= 0) return;
            var randomNumber = Random.Range(0, applicableEffects.Count);
            ApplyStack(enemy, applicableEffects[randomNumber]);
        }
        
        private void ApplyStack(GameObject enemy, (EffectManager.Effect effect, int level) effectToApply)
        {
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    if (!enemyController.stacks.Contains((EffectManager.Effect.None, 0))) return;
                    for (var i = 0; i < enemyController.stacks.Length; i++)
                    {
                        if (enemyController.stacks[i].effect != EffectManager.Effect.None) continue;
                        enemyController.stacks[i] = effectToApply;
                        enemyController.stackTimers[i] = effectManager.effectDuration;
                        break;
                    }
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    if (!bossController.stacks.Contains((EffectManager.Effect.None, 0))) return;
                    for (var i = 0; i < bossController.stacks.Length; i++)
                    {
                        if (bossController.stacks[i].effect != EffectManager.Effect.None) continue;
                        bossController.stacks[i] = effectToApply;
                        bossController.stackTimers[i] = effectManager.effectDuration;
                        break;
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
