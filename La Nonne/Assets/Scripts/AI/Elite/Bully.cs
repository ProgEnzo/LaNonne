using System;
using System.Collections;
using Controller;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Elite
{
    public class Bully : EnemyController
    {
        [Header("Enemy Attack")]
        [SerializeField] private int bullyDamage;
        [SerializeField] private float knockbackPower;
        
        protected override void Update()
        {
            base.Update();
            EnemyDeath();
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(bullyDamage); //Player takes damage

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * knockbackPower;
            
                playerController.mRigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
    }
}
