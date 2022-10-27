using System.Collections;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

public class TrashMobRange : MonoBehaviour
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

        [Header("Enemy Health")]
        [SerializeField] public float currentHealth;
        
        [Header("Enemy Components")]
        [SerializeField] GameObject player;
        private AIPath scriptAIPath;
        [SerializeField] GameObject bulletPrefab;
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;
        public PlayerController playerController;
        
        public bool isStunned;
    

        private void Start()
        {
            scriptAIPath = GetComponent<AIPath>();
            currentHealth = soEnemy.maxHealth;
            isStunned = false;
        }
        private void Update()
        {
            if (!isStunned)
            {
                StopAndShoot();
            }
            HealCeiling();
        }

        #region EnemyRangeBehavior
        void StopAndShoot()
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position); //Ca chope la distance entre le joueur et l'enemy
        
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

            Vector3 position = transform.position;
            Vector3 direction = player.transform.position - position; // direction entre player et enemy
            GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity); // spawn bullet
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>(); // chope le rb de la bullet 
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

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * knockbackBody;
            
                playerController.m_rigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }

        IEnumerator PlayerIsHit()
        {
            playerController.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerController.GetComponent<SpriteRenderer>().color = Color.white;
        }
    
        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.DrawWireSphere(position, shootingRange);
            Gizmos.DrawWireSphere(position, aggroRange);
        }
        
        

        #endregion
    
        #region HealthEnemyRange
        public void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                TrashMobRangeDie();
            }
        }

        private void TrashMobRangeDie()
        {
            Destroy(gameObject);
        }

        private void HealCeiling()
        {
            if (currentHealth > soEnemy.maxHealth)
            {
                currentHealth = soEnemy.maxHealth;
            }
        }

        #endregion
    }

