using System.Collections.Generic;
using System.Linq;
using AI;
using Shop;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class Blade : MonoBehaviour
    {
        public bool isHitting;
        public float hitLength;
        public float hitAngle = 45f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public LineRenderer lineRenderer;
        public BoxCollider2D boxCollider;
        public Quaternion finalRotation1;
        public Quaternion finalRotation2;
        private PlayerController playerController;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private int hitState;
        [SerializeField] private int maxHitState;
        [SerializeField] private float maxDuringComboCooldown;
        private float currentDuringComboCooldown;
        [SerializeField] private float maxNextComboCooldown;
        private float currentNextComboCooldown;
        [SerializeField] private float maxDetectionAngle;
        [SerializeField] private float littleHitStopDuration;
        [SerializeField] private float littleKnockBackForce;
        [SerializeField] private float bigHitStopDuration;
        [SerializeField] private float bigKnockBackForce;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        private EffectManager effectManager;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            effectManager = EffectManager.instance;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
            currentDuringComboCooldown = maxDuringComboCooldown;
            currentNextComboCooldown = maxNextComboCooldown;
            hitState = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            ZealousBlade();
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            lineRenderer.SetPosition(1, new Vector3(0, hitLength/parentLocalScaleX, 0));
            boxCollider.size = new Vector2(0.1f/parentLocalScaleX, hitLength/parentLocalScaleX);
            boxCollider.offset = new Vector2(0, hitLength/parentLocalScaleX/2);
        }

        private void ZealousBlade()
        {
            currentDuringComboCooldown -= Time.deltaTime;
            currentNextComboCooldown -= Time.deltaTime;
            if (currentDuringComboCooldown <= 0)
            {
                hitState = 0;
            }

            if (Input.GetMouseButtonDown(0) && !isHitting && currentNextComboCooldown <= 0)
            {
                hitState += 1;
                currentDuringComboCooldown = maxDuringComboCooldown;
                
                var facingDirection = (playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), playerController.transform.localScale.x) switch
                {
                    (0, > 0 or < 0) => Vector2.down,
                    (1, > 0 or < 0) => Vector2.up,
                    (2, > 0) => Vector2.right,
                    (2, < 0) => Vector2.left,
                    _ => Vector2.right
                };
                transform.rotation = Quaternion.LookRotation(Vector3.forward, facingDirection);
                GameObject enemyToAim = null;
                var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'attaque
                Physics2D.CircleCast(transform.position, hitLength, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'attaque
                if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
                {
                    foreach (var hit in from hit in objectsInArea where hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss") let enemyToAimPosition = hit.collider.transform.position let playerPosition = playerController.transform.position let directionToAim = enemyToAimPosition - playerPosition let angleToAim = Vector2.Angle(facingDirection, directionToAim) where angleToAim <= maxDetectionAngle/2 select hit)
                    {
                        enemyToAim = hit.collider.gameObject;
                        break; //On sort de la boucle
                    }
                }

                Vector3 newDirection;
                if (enemyToAim)
                {
                    newDirection = enemyToAim.transform.position - transform.position;
                    newDirection.z = 0;
                    newDirection.Normalize();
                }
                else
                {
                    newDirection = facingDirection;
                }
                
                var newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                AnimationControllerBool(IsAttacking);
                finalRotation1 = newRotation * Quaternion.Euler(0, 0, hitAngle / 2);
                finalRotation2 = newRotation * Quaternion.Euler(0, 0, -hitAngle / 2);
                lineRenderer.enabled = true;
                boxCollider.enabled = true;
                isHitting = true;

                if (hitState % 2 == 1)
                {
                    transform.rotation = finalRotation2;
                }
                else
                {
                    transform.rotation = finalRotation1;
                }
            }

            if (isHitting)
            {
                if (hitState % 2 == 1)
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation1, hitSpeed * Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation1.eulerAngles.z + toleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation1.eulerAngles.z - toleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        AnimationManagerBool(IsAttacking, false);
                        playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                        if (hitState == maxHitState)
                        {
                            currentNextComboCooldown = maxNextComboCooldown;
                            hitState = 0;
                        }
                    }
                }
                else
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation2, hitSpeed * Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation2.eulerAngles.z + toleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation2.eulerAngles.z - toleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        AnimationManagerBool(IsAttacking, false);
                        playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                        if (hitState == maxHitState)
                        {
                            currentNextComboCooldown = maxNextComboCooldown;
                            hitState = 0;
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //DMG du player sur les enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.playerAttackDamage);
                if (hitState == maxHitState)
                {
                    other.gameObject.GetComponent<EnemyController>().HitStopAndKnockBack(bigHitStopDuration, bigKnockBackForce);
                }
                else
                {
                    other.gameObject.GetComponent<EnemyController>().HitStopAndKnockBack(littleHitStopDuration, littleKnockBackForce);
                }
                PutStack(other.gameObject);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }
            
            //DMG du player sur le BOSS
            if (other.gameObject.CompareTag("Boss"))
            {
                other.gameObject.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(soController.playerAttackDamage);
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
            var enemyController = enemy.GetComponent<EnemyController>();
            if (!enemyController.stacks.Contains((EffectManager.Effect.None, 0))) return;
            for (var i = 0; i < enemyController.stacks.Length; i++)
            {
                if (enemyController.stacks[i].effect != EffectManager.Effect.None) continue;
                enemyController.stacks[i] = effectToApply;
                enemyController.stackTimers[i] = effectManager.effectDuration;
                break;
            }
        }
        
        private void AnimationControllerBool(int parameterToChange)
        {
            AnimationManagerBool(parameterToChange, true);
            StartCoroutine(playerController.CanChangeCoroutine());
        }
        
        private void AnimationManagerBool(int parameterToChange, bool value)
        {
            if (parameterToChange  == IsAttacking)
            {
                playerController.AnimationManagerSwitch(playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), playerController.currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }

            foreach (var prefab in playerController.animPrefabs.Where(prefab => prefab != playerController.currentAnimPrefab))
            {
                prefab.SetActive(false);
            }
        }
    }
}
