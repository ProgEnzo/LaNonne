using System.Collections;
using System.Collections.Generic;
using Controller;
using DG.Tweening;
using Pathfinding;
using Shop;
using Tools;
using UnityEngine;

namespace AI
{
    public class EnemyController : MonoBehaviour
    {
        protected PlayerController playerController;
        [SerializeField] protected SO_Enemy soEnemy;
        [SerializeField, ShowOnly] internal float currentHealth;
        [SerializeField, ShowOnly] internal float currentAiPathSpeed;
        [SerializeField, ShowOnly] internal float currentVelocitySpeed;
        [SerializeField, ShowOnly] internal float currentDamageMultiplier;
        [SerializeField, ShowOnly] internal float currentEpDropMultiplier;
        //eds

        /*[NonSerialized]*/ public bool isStunned;
        private AIPath aiPathComponent;
        protected Rigidbody2D rb;
        private static readonly List<SpriteRenderer> PlayerSpriteRenderers = new();
        [SerializeField, ShowOnly] internal (EffectManager.Effect effect, int level)[] stacks = new (EffectManager.Effect, int)[3];
        [SerializeField, ShowOnly] internal float[] stackTimers = new float[3];
        [SerializeField, ShowOnly] internal bool[] areStacksOn = new bool[3];
        private EffectManager effectManager;
        private Coroutine currentHitStopCoroutine;

        public GameObject epDrop;
        public int numberOfEp;

        protected virtual void Start()
        {
            aiPathComponent = GetComponent<AIPath>();
            rb = GetComponent<Rigidbody2D>();
            playerController = PlayerController.instance;
            effectManager = EffectManager.instance;
            currentHealth = soEnemy.maxHealth;
            currentAiPathSpeed = soEnemy.aiPathBasicSpeed;
            currentVelocitySpeed = soEnemy.velocityBasicSpeed;
            currentDamageMultiplier = 1f;
            currentEpDropMultiplier = 1f;
            if (GetComponent<AIDestinationSetter>() != null)
            {
                GetComponent<AIDestinationSetter>().target = playerController.transform;
            }
            isStunned = false;
            
            for (var i = 0; i < stacks.Length; i++)
            {
                stacks[i].effect = EffectManager.Effect.None; 
                stacks[i].level = 0;
                stackTimers[i] = 0;
                areStacksOn[i] = false;
            }
        }
        
        protected virtual void Update()
        {
            aiPathComponent.maxSpeed = currentAiPathSpeed;
            
            PlayerSpriteRenderers.Clear();
            for (var i = 0; i < playerController.transform.childCount; i++)
            {
                if (playerController.transform.GetChild(i).TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    PlayerSpriteRenderers.Add(spriteRenderer);
                }
            }

            HealCeiling();
            StunCheck();
            EffectCheck();
        }
        
        #region HealthEnemy
        public virtual void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage * currentDamageMultiplier;
            EnemyDeath();
        }

        protected void EnemyDeath()
        {
            if (currentHealth <= 0)
            {
                for (int i = 0; i < numberOfEp; i++)
                {
                    var epPos = transform.position;
                    var epDropObject = Instantiate(epDrop, new Vector2(epPos.x, epPos.y), Quaternion.identity);

                    epDropObject.transform.DOMove(new Vector2(epPos.x + Random.Range(-1f, 0f), epPos.y + Random.Range(-0, 1f)), 0.5f).SetEase(Ease.OutBounce);
                }

                Destroy(gameObject); //Dies            
            }
        }

        

        private void HealCeiling()
        {
            if (currentHealth > soEnemy.maxHealth)
            {
                currentHealth = soEnemy.maxHealth;
            }
        }
        #endregion
        
        internal void HitStopAndKnockBack(float hitStopDuration, float knockBackForce)
        {
            if (currentHitStopCoroutine != null)
            {
                StopCoroutine(currentHitStopCoroutine);
            }
            currentHitStopCoroutine = StartCoroutine(HitStop(hitStopDuration));
            rb.AddForce((transform.position - playerController.transform.position).normalized * knockBackForce, ForceMode2D.Impulse);
        }
        
        private IEnumerator HitStop(float hitStopDuration)
        {
            var velocitySpeed = currentVelocitySpeed;
            aiPathComponent.canMove = false;
            currentVelocitySpeed = 0;
            yield return new WaitForSeconds(hitStopDuration);
            currentVelocitySpeed = velocitySpeed;
            aiPathComponent.canMove = true;
            currentHitStopCoroutine = null;
        }

        protected virtual void StunCheck()
        {
            aiPathComponent.enabled = !isStunned;
        }
        
        private void EffectCheck()
        {
            for (var i = 0; i < stacks.Length; i++)
            {
                if (stacks[i].effect == EffectManager.Effect.None)
                {
                    continue;
                }
                stackTimers[i] -= Time.deltaTime;
                if (stackTimers[i] <= 0)
                {
                    stackTimers[i] = 0;
                    stacks[i].effect = EffectManager.Effect.None;
                    stacks[i].level = 0;
                    continue;
                }

                if (areStacksOn[i]) continue;
                areStacksOn[i] = true;
                effectManager.EffectSwitch(stacks[i].effect, stacks[i].level, gameObject, i);
            }
        }

        protected internal static IEnumerator PlayerIsHit()
        {
            foreach (var spriteRenderer in PlayerSpriteRenderers)
            {
                spriteRenderer.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (var spriteRenderer in PlayerSpriteRenderers)
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
