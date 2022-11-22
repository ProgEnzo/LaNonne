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
        protected PlayerController playerController;
        [SerializeField] protected SO_Enemy soEnemy;
        [SerializeField] public float currentHealth;

        [NonSerialized] public bool isStunned;
        private AIPath aiPathComponent;
        private static readonly List<SpriteRenderer> PlayerSpriteRenderers = new();

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
                spriteRenderer.color = Color.white;
            }
        }
    }
}
