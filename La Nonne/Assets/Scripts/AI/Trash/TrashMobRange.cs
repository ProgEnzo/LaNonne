using System;
using System.Collections;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Trash
{
    public class TrashMobRange : EnemyController
    {
        [Header("Enemy Detection")]
        [SerializeField] float distanceToPlayer;
        [SerializeField] float shootingRange;
        [SerializeField] float aggroRange;
        private float cooldownTimer;
    
        [Header("Enemy Attack Values")]
        [SerializeField] float cooldownBetweenShots;
        [SerializeField] float bulletSpeed;
        [SerializeField] float knockbackBody;
        
        [Header("Enemy Components")]
        private AIPath scriptAIPath;
        [SerializeField] private GameObject bulletPrefab;

        protected override void Start()
        {
            base.Start();
            scriptAIPath = GetComponent<AIPath>();
        }
        
        protected override void Update()
        {
            base.Update();
            EnemyDeath();

            if (!isStunned)
            {
                StopAndShoot();
            }
        }

        #region EnemyRangeBehavior

        private void StopAndShoot()
        {
            distanceToPlayer = Vector2.Distance(playerController.transform.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
            if (distanceToPlayer <= shootingRange) //si le joueur est dans la SHOOTING RANGE du trashMob
            {
                scriptAIPath.maxSpeed = 3;
                Shoot();
            
            }
            else if (distanceToPlayer <= aggroRange) //si le joueur est dans l'AGGRO RANGE du trashMob
            {
                scriptAIPath.maxSpeed = 6;
            
            }
            else if (distanceToPlayer > aggroRange) //si le joueur est HORS de l'AGGRO RANGE
            {
                scriptAIPath.maxSpeed = 0;
            }
        }
    
        private void Shoot()
        {
            //Timer for firing bullets
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer > 0)
            {
                return;
            }
            cooldownTimer = cooldownBetweenShots;

            var position = transform.position;
            var direction = playerController.transform.position - position; // direction entre player et enemy
            var bullet = Instantiate(bulletPrefab, position, Quaternion.identity); // spawn bullet
            var rbBullet = bullet.GetComponent<Rigidbody2D>(); // chope le rb de la bullet 
            rbBullet.AddForce(direction * bulletSpeed, ForceMode2D.Impulse); // Addforce avec la direction + le rb
        
            Destroy(bullet, 3f);
        }
        
        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le TrashMobRange touche le player
            if (col.gameObject.CompareTag("Player") && !isStunned)
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(soEnemy.bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockback = direction * knockbackBody;
            
                playerController.mRigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, shootingRange);
            Gizmos.DrawWireSphere(position, aggroRange);
        }
        #endregion
    }
}

