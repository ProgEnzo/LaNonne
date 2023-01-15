using System.Collections;
using AI.So;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
// ReSharper disable CommentTypo

namespace AI.Trash
{
    public class TrashMobRange : EnemyController
    {
        [Header("Enemy Detection")]
        private float distanceToPlayer;
        private float cooldownTimer;
        
        [Header("Enemy Components")]
        private AIPath scriptAIPath;
        [SerializeField] private GameObject bulletPrefab;
        private SoTrashMobRange soTrashMobRange;
        [SerializeField] private Animator animator;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        protected override void Start()
        {
            base.Start();
            soTrashMobRange = (SoTrashMobRange) soEnemy;
            scriptAIPath = GetComponent<AIPath>();
        }
        
        protected override void Update()
        {
            base.Update();

            if (!isStunned)
            {
                StopAndShoot();
            }
        }

        #region EnemyRangeBehavior

        private void StopAndShoot()
        {
            distanceToPlayer = Vector2.Distance(playerController.transform.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
            if (distanceToPlayer <= soTrashMobRange.maxProxRange)
            {
                scriptAIPath.maxSpeed = 0;
                
                Shoot();
            }
            else if (distanceToPlayer <= soTrashMobRange.shootingRange) //si le joueur est dans la SHOOTING RANGE du trashMob
            {
                scriptAIPath.maxSpeed = 1;
                
                Shoot();
            }
            else if (distanceToPlayer <= soTrashMobRange.aggroRange) //si le joueur est dans l'AGGRO RANGE du trashMob
            {
                scriptAIPath.maxSpeed = 3;
            
            }
            else if (distanceToPlayer > soTrashMobRange.aggroRange) //si le joueur est HORS de l'AGGRO RANGE
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
            cooldownTimer = soTrashMobRange.cooldownBetweenShots;
            
            animator.SetBool(IsAttacking, true);

            var position = transform.position;
            var direction = playerController.transform.position - position; // direction entre player et enemy
            var bullet = Instantiate(bulletPrefab, position, Quaternion.identity); // spawn bullet
            var rbBullet = bullet.GetComponent<Rigidbody2D>(); // chope le rb de la bullet 
            rbBullet.AddForce(direction.normalized * soTrashMobRange.bulletSpeed, ForceMode2D.Impulse); // Addforce avec la direction + le rb

            StartCoroutine(AnimationAttackFalse());
            
            Destroy(bullet, 3f);
        }
        
        private IEnumerator AnimationAttackFalse()
        {
            yield return new WaitForNextFrameUnit();
            animator.SetBool(IsAttacking, false);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, soTrashMobRange.shootingRange);
            Gizmos.DrawWireSphere(position, soTrashMobRange.aggroRange);
        }
        
        #endregion
    }
}

