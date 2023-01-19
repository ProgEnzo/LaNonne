using System;
using System.Collections;
using AI.So;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.Elite
{
    public class Bully : EnemyController
    {
        private SoBully soBully;
        private Animator animator;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        private void Awake()
        {
            soBully = (SoBully) soEnemy;
            animator = transform.GetChild(2).GetComponent<Animator>();
        }

        protected override void Update()
        {
            base.Update();
            
            CheckDirection();
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned && !playerController.isRevealingDashOn)
            {
                animator.SetBool(IsAttacking, true);
                StartCoroutine(AnimationAttackFalse());
                
                playerController.TakeDamage(soBully.bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * soBully.knockBackPower;
            
                playerController.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
            }
        }
        
        private IEnumerator AnimationAttackFalse()
        {
            yield return new WaitForNextFrameUnit();
            animator.SetBool(IsAttacking, false);
        }
        
        private void CheckDirection()
        {
            var puppetLocalScale = enemyPuppet.transform.localScale;
            if (rb.velocity.x != 0)
            {
                enemyPuppet.transform.localScale = new Vector3(MathF.Sign(rb.velocity.x) * MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
            else
            {
                enemyPuppet.transform.localScale = playerController.transform.position.x > transform.position.x ? new Vector3(MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z) : new Vector3(-MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
        }
    }
}
