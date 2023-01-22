using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.So;
using Controller;
using DG.Tweening;
using Manager;
using Pathfinding;
using Shop;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class EnemyController : MonoBehaviour
    {
        protected PlayerController playerController;
        [SerializeField] internal SO_Enemy soEnemy;
        [SerializeField] internal GameObject effectVFXGameObject;
        public ScoreManager scoreManager;
        internal float currentHealth;
        internal float currentAiPathSpeed;
        internal float currentVelocitySpeed;
        internal float currentDamageMultiplier;
        internal float currentEpDropMultiplier;

        internal bool isStunned;
        protected bool isDead;
        private AIPath aiPathComponent;
        protected Rigidbody2D rb;
        internal readonly (EffectManager.Effect effect, int level)[] stacks = new (EffectManager.Effect, int)[3];
        internal readonly float[] stackTimers = new float[3];
        internal readonly bool[] areStacksOn = new bool[3];
        private EffectManager effectManager;
        private Coroutine currentHitStopCoroutine;
        private float lastVelocitySpeed;
        protected bool isKnockedBack;

        [SerializeField] private GameObject epDrop;
        private List<SpriteRenderer> enemySpriteRenderers = new();
        [SerializeField] protected GameObject enemyPuppet;
        protected Coroutine currentIsHitCoroutine;

        [SerializeField] private GameObject bloodDroplets;
        
        [Header("SoundEffect")]
        [SerializeField] protected AudioSource hitAudioSource;
        [SerializeField] protected AudioClip[] hitRandomSound;

        protected virtual void Start()
        {
            aiPathComponent = GetComponent<AIPath>();
            rb = GetComponent<Rigidbody2D>();
            playerController = PlayerController.instance;
            effectManager = EffectManager.instance;
            scoreManager = ScoreManager.instance;
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
            isKnockedBack = false;
            
            for (var i = 0; i < stacks.Length; i++)
            {
                stacks[i].effect = EffectManager.Effect.None; 
                stacks[i].level = 0;
                stackTimers[i] = 0;
                areStacksOn[i] = false;
            }
            
            enemySpriteRenderers = enemyPuppet.GetComponentsInChildren<SpriteRenderer>(true).ToList();
        }
        
        protected virtual void Update()
        {
            aiPathComponent.maxSpeed = currentAiPathSpeed;
            
            if (isKnockedBack && rb.velocity.magnitude < 0.01f)
            {
                isKnockedBack = false;
            }

            HealCeiling();
            StunCheck();
            EffectCheck();
        }
        
        #region HealthEnemy
        public virtual void TakeDamageFromPlayer(int damage)
        {
            hitAudioSource.PlayOneShot(hitRandomSound[Random.Range(0, hitRandomSound.Length)]);
            var dropletBloodObject = Instantiate(bloodDroplets, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Quaternion.identity);

            dropletBloodObject.GetComponent<ParticleSystem>().Play();
            Destroy(dropletBloodObject, 2f);

            currentHealth -= damage * currentDamageMultiplier;
            EnemyDeath();
            
            if (currentIsHitCoroutine != null)
            {
                StopCoroutine(currentIsHitCoroutine);
            }
            currentIsHitCoroutine = StartCoroutine(EnemyIsHit());
        }

        protected virtual void EnemyDeath()
        {
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                
                EpDrop((int)(soEnemy.numberOfEp * currentEpDropMultiplier));
                
                scoreManager.AddKilledEnemyScore(soEnemy.scorePoint);

                Destroy(gameObject); //Dies            
            }
        }

        internal void EpDrop(int epNumber)
        {
            for (var i = 0; i < epNumber; i++)
            {
                var epPos = transform.position;
                var epDropObject = Instantiate(epDrop, new Vector2(epPos.x, epPos.y), Quaternion.identity);

                epDropObject.transform.DOMove(new Vector2(epPos.x + Random.Range(-1f, 0f), epPos.y + Random.Range(-0, 1f)), 0.5f).SetEase(Ease.OutBounce);
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
        
        internal virtual void HitStopAndKnockBack(float hitStopDuration, float knockBackForce)
        {
            isKnockedBack = true;
            rb.AddForce((transform.position - playerController.transform.position).normalized * knockBackForce, ForceMode2D.Impulse);
            HitStop(hitStopDuration);
        }

        internal virtual void HitStop(float hitStopDuration)
        {
            if (currentHitStopCoroutine != null)
            {
                StopCoroutine(currentHitStopCoroutine);
                currentVelocitySpeed = lastVelocitySpeed;
            }
            currentHitStopCoroutine = StartCoroutine(HitStopCoroutine(hitStopDuration));
        }
        
        private IEnumerator HitStopCoroutine(float hitStopDuration)
        {
            lastVelocitySpeed = currentVelocitySpeed;
            aiPathComponent.canMove = false;
            currentVelocitySpeed = 0;
            yield return new WaitForSeconds(hitStopDuration);
            currentVelocitySpeed = lastVelocitySpeed;
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

        protected IEnumerator EnemyIsHit()
        {
            foreach (var spriteRenderer in enemySpriteRenderers)
            {
                spriteRenderer.color = Color.red;
            }
            yield return new WaitForSeconds(0.2f);
            foreach (var spriteRenderer in enemySpriteRenderers)
            {
                spriteRenderer.color = Color.white;
            }
        }
        
        protected void CheckDirection(bool invert = false)
        {
            var invertValue = invert ? -1 : 1;
            var puppetLocalScale = enemyPuppet.transform.localScale;
            enemyPuppet.transform.localScale = playerController.transform.position.x > transform.position.x ? new Vector3(MathF.Abs(puppetLocalScale.x) * invertValue, puppetLocalScale.y, puppetLocalScale.z) : new Vector3(-MathF.Abs(puppetLocalScale.x) * invertValue, puppetLocalScale.y, puppetLocalScale.z);
        }
    }
}
