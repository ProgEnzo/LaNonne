using System;
using AI.So;
using UnityEngine;

namespace AI.Trash
{
    public class TrashMobClose : EnemyController
    {
        private SoTrashMobClose soTrashMobClose;

        private void Awake()
        {
            soTrashMobClose = (SoTrashMobClose) soEnemy;
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le TrashMobClose touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(soEnemy.bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * soTrashMobClose.knockBackPower;
            
                playerController.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
            }
        }
    }
}
