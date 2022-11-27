using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Pathfinding;
using Shop;
using UnityEngine;

namespace AI
{
    public class EnemyController : MonoBehaviour
    {
        protected PlayerController playerController;
        [SerializeField] protected SO_Enemy soEnemy;
        [SerializeField] public float currentHealth;

        /*[NonSerialized]*/ public bool isStunned;
        private AIPath aiPathComponent;
        private static readonly List<SpriteRenderer> PlayerSpriteRenderers = new();
        internal readonly (EffectManager.Effect effect, int level)[] stacks = new (EffectManager.Effect, int)[3];
        internal readonly float[] stackTimers = new float[3];

        protected virtual void Start()
        {
            aiPathComponent = GetComponent<AIPath>();
            playerController = PlayerController.instance;
            currentHealth = soEnemy.maxHealth;
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
            }
        }
        
        protected virtual void Update()
        {
            playerController = PlayerController.instance;
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
            currentHealth -= damage;
            EnemyDeath();
        }

        protected void EnemyDeath()
        {
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
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
                    stacks[i].effect = EffectManager.Effect.None;
                    stacks[i].level = 0;
                }
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
