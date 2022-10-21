using System;
using System.Collections;
using Controller;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable CommentTypo

namespace AI.Elite
{
    public class Pyromaniac : MonoBehaviour
    {
        [Header("Enemy Health")]
        [SerializeField] public float currentHealth;
        [SerializeField] public float cooldownTimer;
        private float timer;
        [SerializeField] public float detectionRadius;
        [SerializeField] public float throwRadius;
        [SerializeField] public float projectileSpeed;
        [SerializeField] public float dashSpeed = 10f;
        private Coroutine currentCoroutine;
        private AIPath scriptAIPath;
        private bool isDashing;

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        public bool isStunned;
        
        private void Start()
        {
            scriptAIPath = GetComponent<AIPath>();
            currentHealth = soEnemy.maxHealth;
            timer = cooldownTimer;
            isDashing = false;
        }

        private void Update()
        {
            //Accès à la variable statique du PlayerController
            var playerRef = PlayerController.instance;
            
            //Initialisation de variables locales pour l'optimisation
            var playerPosition = playerRef.transform.position; //Position du joueur
            var transform1 = transform;
            var position = transform1.position; //Position du pyromane
            var projectile = transform1.GetChild(0).gameObject;
            var projectileScript = projectile.GetComponent<PyromaniacProjectile>();
            
            //Tant que le projectile n'a pas explosé
            if (!projectileScript.isExploded)
            {
                //Si le joueur est dans le rayon de détection
                if (Vector3.Distance(position, playerPosition) <= detectionRadius)
                {
                    scriptAIPath.maxSpeed = 0f;
                    GetComponent<AIDestinationSetter>().enabled = false;
                    scriptAIPath.enabled = false;
                    //Sinon, le pyromane lance sa zone de feu
                    var distanceToCross = Mathf.Min(Vector3.Distance(position, playerPosition), throwRadius); //On calcule la distance à parcourir par le projectile. On prend la distance entre le joueur et le pyromane, et on la limite à la distance maximale de lancer du projectile.
                    var newPositionVector = (playerPosition - position).normalized * distanceToCross; //On calcule le vecteur de déplacement du projectile
                    var newPosition = position + newPositionVector; //On calcule la nouvelle position du projectile
                    ThrowProjectile(newPosition, newPositionVector.normalized); //On lance le projectile à la nouvelle position, avec la nouvelle direction
                }
                else
                {
                    //Déplacement du pyromane
                    scriptAIPath.maxSpeed = 3f;
                }
            }
            //Une fois que le projectile a explosé
            else
            {
                //Dash vers la zone de feu
                currentCoroutine ??= StartCoroutine(DashToFireZone());
            }
        }

        private void ThrowProjectile(Vector3 destination, Vector3 direction)
        {
            var transform1 = transform;
            var projectile = transform1.GetChild(0).gameObject;
            projectile.transform.position = transform1.position;
            projectile.SetActive(true);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            projectile.GetComponent<PyromaniacProjectile>().destination = destination;
        }
        
        private IEnumerator DashToFireZone()
        {
            yield return new WaitForSeconds(0.5f);
            var transform1 = transform;
            var position = transform1.position; //Position du pyromane
            var projectile = transform1.GetChild(0).gameObject;
            isDashing = true;
            GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - position).normalized * dashSpeed);
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, detectionRadius);
            Gizmos.DrawWireSphere(position, throwRadius);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (isDashing)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                isDashing = false;
            }
        }
    }
}