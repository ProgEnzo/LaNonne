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

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
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
    }
}
