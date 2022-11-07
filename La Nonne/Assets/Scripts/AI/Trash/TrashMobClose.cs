using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Trash
{
    public class TrashMobClose : EnemyController
    {
        [Header("Enemy Attack")]
        [SerializeField] private float knockbackPower;

        [Header("Enemy Components")]
        public PlayerController playerController;

        protected override void Update()
        {
            base.Update();
            EnemyDeath();
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le TrashMobClose touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(soEnemy.bodyDamage); //Player takes damage

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * knockbackPower;
            
                playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
    }
}
