using System;
using Controller;
using UnityEngine;

namespace AI.Elite
{
    public class Autophagic : EnemyController
    {
        [SerializeField] private float detectionRadius;
        [SerializeField] private float autoDamagePart;
        [SerializeField] private float maxTimer;
        private float currentTimer;
        private bool isActivated;

        protected override void Update()
        {
            base.Update();
            EnemyDeath();
            
            if (!isActivated)
            {
                var distanceToPlayer = Vector2.Distance(playerController.transform.position, transform.position);
                if (distanceToPlayer < detectionRadius)
                {
                    isActivated = true;
                }
            }
            else
            {
                if (currentTimer <= 0)
                {
                    TakeAutoDamage();
                    currentTimer = maxTimer;
                }
                currentTimer -= Time.deltaTime;
            }
        }
        
        private void TakeAutoDamage()
        {
            currentHealth -= soEnemy.maxHealth * autoDamagePart;
        }
    }
}
