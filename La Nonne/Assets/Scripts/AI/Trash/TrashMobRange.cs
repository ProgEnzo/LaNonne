using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Trash
{
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

        [Header("Enemy Components")]
        [SerializeField] GameObject player;
        private AIPath scriptAIPath;
        [SerializeField] GameObject bulletPrefab;
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;


        [Header("Enemy Health")] 
        [SerializeField] public float currentHealth;
    
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

        #endregion
    }
}
