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

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        public bool isStunned;
        
        private void Start()
        {
            currentHealth = soEnemy.maxHealth;
            timer = cooldownTimer;
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
                    //Si le timer n'est pas écoulé, on le décrémente
                    if (timer > 0f)
                    {
                        //timer -= Time.deltaTime;
                    }
                    //Sinon, le pyromane lance sa zone de feu
                    else
                    {
                        var distanceToCross = Mathf.Min(Vector3.Distance(position, playerPosition), throwRadius); //On calcule la distance à parcourir par le projectile. On prend la distance entre le joueur et le pyromane, et on la limite à la distance maximale de lancer du projectile.
                        var newPositionVector = (playerPosition - position).normalized * distanceToCross; //On calcule le vecteur de déplacement du projectile
                        var newPosition = position + newPositionVector; //On calcule la nouvelle position du projectile
                        ThrowProjectile(newPosition, newPositionVector.normalized); //On lance le projectile à la nouvelle position, avec la nouvelle direction
                
                        //Réinitialisation du timer
                        timer = 10f;
                    }
                }
                else
                {
                    //Déplacement du pyromane
                }
            }
            //Une fois que le projectile a explosé
            else
            {
                //Constat du feu
                currentCoroutine ??= StartCoroutine(WaitToDash());
                
                //Dash vers la zone de feu
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
        
        private IEnumerator WaitToDash()
        {
            yield return new WaitForSeconds(0.5f);
            GetComponent<AIDestinationSetter>().enabled = false;
            GetComponent<AIPath>().enabled = false;
            var transform1 = transform;
            var position = transform1.position; //Position du pyromane
            var projectile = transform1.GetChild(0).gameObject;
            GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - position).normalized * dashSpeed);
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, detectionRadius);
            Gizmos.DrawWireSphere(position, throwRadius);
        }
    }
}