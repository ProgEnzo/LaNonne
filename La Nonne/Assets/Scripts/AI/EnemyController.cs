using System;
using System.Collections;
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
        private static SpriteRenderer _playerSpriteRenderer;

        protected virtual void Start()
        {
            aiPathComponent = GetComponent<AIPath>();
            var playerRef = PlayerController.instance;
            _playerSpriteRenderer = playerRef.GetComponent<SpriteRenderer>();
            
            currentHealth = soEnemy.maxHealth;
            GetComponent<AIDestinationSetter>().target = playerRef.transform;
            isStunned = false;
        }
        
        protected virtual void Update()
        {
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

        private void StunCheck()
        {
            aiPathComponent.enabled = !isStunned;
        }

        protected internal static IEnumerator PlayerIsHit()
        {
            _playerSpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _playerSpriteRenderer.color = Color.yellow;
        }
    }
}
