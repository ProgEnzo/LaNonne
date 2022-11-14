using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Pathfinding;
using UnityEngine;

namespace AI
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] protected SO_Enemy soEnemy;
        [SerializeField] public float currentHealth;

        [NonSerialized] public bool isStunned;
        private AIPath aiPathComponent;
        private static readonly List<SpriteRenderer> PlayerSpriteRenderers = new();

        protected virtual void Start()
        {
            aiPathComponent = GetComponent<AIPath>();
            var playerRef = PlayerController.instance;
            currentHealth = soEnemy.maxHealth;
            GetComponent<AIDestinationSetter>().target = playerRef.transform;
            isStunned = false;
        }
        
        protected virtual void Update()
        {
            var playerRef = PlayerController.instance;
            PlayerSpriteRenderers.Clear();
            for (var i = 0; i < playerRef.transform.childCount; i++)
            {
                if (playerRef.transform.GetChild(i).TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    PlayerSpriteRenderers.Add(spriteRenderer);
                }
            }
            
            HealCeiling();
            StunCheck();
        }
        
        #region HealthEnemy
        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;
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

        protected internal static IEnumerator PlayerIsHit()
        {
            foreach (var spriteRenderer in PlayerSpriteRenderers)
            {
                spriteRenderer.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);
            foreach (var spriteRenderer in PlayerSpriteRenderers)
            {
                spriteRenderer.color = Color.yellow;
            }
        }
    }
}
