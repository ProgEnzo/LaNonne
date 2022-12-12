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

        private void Start()
        {
            playerController = PlayerController.instance;
            effectManager = EffectManager.instance;
            animationManager = AnimationManager.instance;
            inputManager = InputManager.instance;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
            currentDuringComboCooldown = 0;
            currentNextComboCooldown = 0;
            hitState = 0;
        }

        private void Update()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            currentDuringComboCooldown -= Time.deltaTime;
            currentNextComboCooldown -= Time.deltaTime;
            if (currentDuringComboCooldown <= 0)
            {
                hitState = 0;
            }

            if (Input.GetKeyDown(inputManager.zealousBladeKey) && !playerController.isSlowMoOn && !isHitting && currentNextComboCooldown <= 0)
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
                        Quaternion.RotateTowards(transform.rotation, finalRotation1, soController.zealousBladeHitSpeed * Time.unscaledDeltaTime * playerController.currentSlowMoPlayerAttackSpeedFactor / Time.timeScale);
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
                        }
                    }
                }
                else
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation2, soController.zealousBladeHitSpeed * Time.unscaledDeltaTime * playerController.currentSlowMoPlayerAttackSpeedFactor / Time.timeScale);
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
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var o = other.gameObject;
            
            //DMG du player sur les enemy
            if (o.CompareTag("Enemy"))
            {
                o.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.zealousBladeDamage);
                if (hitState == soController.zealousBladeMaxHitState)
                {
                    o.GetComponent<EnemyController>().HitStopAndKnockBack(soController.zealousBladeBigHitStopDuration, soController.zealousBladeBigKnockBackForce);
                }
                else
                {
                    o.GetComponent<EnemyController>().HitStopAndKnockBack(soController.zealousBladeLittleHitStopDuration, soController.zealousBladeLittleKnockBackForce);
                }
                PutStack(o);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }
            
            //DMG du player sur le BOSS
            if (o.CompareTag("Boss"))
            {
                o.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(soController.zealousBladeDamage);
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
