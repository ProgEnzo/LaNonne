using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private GameObject circleGameObject;
        [SerializeField] public float detectionRadius;
        [SerializeField] public float throwRadius;
        [SerializeField] public float projectileSpeed;
        [SerializeField] public float dashSpeed = 10f;
        [SerializeField] public float explosionRadius;
        private Vector3 dashInitialPosition;
        private Coroutine currentCoroutine;
        private Coroutine currentHittingCoroutine;
        private AIPath scriptAIPath;
        private bool isDashing;
        private bool isProjectileOn;
        private bool isImpactOn;
        [SerializeField] private float fireCooldown = 1f;

        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        public bool isStunned;
        
        private void Start()
        {
            scriptAIPath = GetComponent<AIPath>();
            currentHealth = soEnemy.maxHealth;
            isDashing = false;
            isProjectileOn = false;
            isImpactOn = false;
            circleGameObject = transform.GetChild(1).gameObject; //Initialisation de l'accès au cercle
            circleGameObject.SetActive(false); //On le désactive pour le moment
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
                    if (isProjectileOn) return;
                    isProjectileOn = true;
                    scriptAIPath.maxSpeed = 0f;
                    GetComponent<AIDestinationSetter>().enabled = false;
                    scriptAIPath.enabled = false;
                    //Sinon, le pyromane lance sa zone de feu
                    var distanceToCross = Mathf.Min(Vector3.Distance(position, playerPosition), throwRadius); //On calcule la distance à parcourir par le projectile. On prend la distance entre le joueur et le pyromane, et on la limite à la distance maximale de lancer du projectile.
                    var newPositionVector = (playerPosition - position).normalized * distanceToCross; //On calcule le vecteur de déplacement du projectile
                    var newPosition = position + newPositionVector; //On calcule la nouvelle position du projectile
                    ThrowProjectile(newPosition, newPositionVector.normalized); //On lance le projectile à la nouvelle position, avec la nouvelle direction
                    Debug.Log(newPosition);
                }
                else if (!isProjectileOn && !isDashing && !isImpactOn)
                {
                    //Déplacement du pyromane
                    GetComponent<AIDestinationSetter>().enabled = true;
                    scriptAIPath.enabled = true;
                    scriptAIPath.maxSpeed = 3f;
                }
            }
            //Une fois que le projectile a explosé
            else
            {
                if (!isDashing && !isImpactOn)
                {
                    isProjectileOn = false;
                    //Dash vers la zone de feu
                    currentCoroutine ??= StartCoroutine(DashToFireZone());
                }
                else
                {
                    var objectsInArea = new List<RaycastHit2D>();
                    Physics2D.BoxCast(dashInitialPosition, new Vector2(3f, 3f), 0f, (position - dashInitialPosition).normalized, new ContactFilter2D(), objectsInArea, Vector2.Distance(dashInitialPosition, position));
                    foreach (var unused in objectsInArea.Where(hit => hit.collider.CompareTag("Player")))
                    {
                        currentHittingCoroutine ??= StartCoroutine(FireDamage());
                    }
                    BoxCastDebug.DrawBoxCast2D(dashInitialPosition, new Vector2(3f, 3f), 0f, (position - dashInitialPosition).normalized, Vector2.Distance(dashInitialPosition, position), Color.magenta);
                }
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
            dashInitialPosition = transform1.position; //Position du pyromane
            var projectile = transform1.GetChild(0).gameObject;
            isDashing = true;
            GetComponent<Rigidbody2D>().AddForce((projectile.transform.position - dashInitialPosition).normalized * dashSpeed);
        }
        
        private IEnumerator FireDamage()
        {
            //Accès à la variable statique du PlayerController
            var playerRef = PlayerController.instance;
            
            StartCoroutine(PlayerIsHit());
            playerRef.TakeDamage(soEnemy.bodyDamage);
            yield return new WaitForSeconds(1f);
            currentHittingCoroutine = null;
        }
        
        private IEnumerator BlinkFire()
        {
            //Render d'un coup de feu
            var circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
            var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
            circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f); //Opacité du cercle
            
            //Dégâts de feu
            var playerRef = PlayerController.instance;
            var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
            Physics2D.CircleCast(transform.position, explosionRadius, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
            if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
            {
                foreach (var unused in objectsInArea.Where(hit => hit.collider.CompareTag("Player")))
                {
                    StartCoroutine(PlayerIsHit());
                    playerRef.TakeDamage(soEnemy.bodyDamage); //Le joueur prend des dégâts
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f); //Transparence du cercle
            yield return new WaitForSeconds(fireCooldown);
            
            circleGameObject.SetActive(false); //On désactive le cercle
            isImpactOn = false;
            var transform1 = transform;
            var projectile = transform1.GetChild(0).gameObject;
            projectile.transform.position = transform1.position;
            projectile.GetComponent<PyromaniacProjectile>().isExploded = false;
            projectile.GetComponent<PyromaniacProjectile>().currentCoroutine = null;
            projectile.SetActive(false);
            
            currentCoroutine = null;
        }

        private static IEnumerator PlayerIsHit()
        {
            var playerRef = PlayerController.instance;
            playerRef.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRef.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, detectionRadius);
            Gizmos.DrawWireSphere(position, throwRadius);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (isDashing)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                isDashing = false;
                isImpactOn = true;
                
                //Cercle d'explosion
                circleGameObject.SetActive(true); //On active le cercle
                circleGameObject.transform.localScale = Vector3.one * (explosionRadius * 8); //On le met à la bonne taille
                var circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
                var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f); //Opacité du cercle

                StartCoroutine(BlinkFire());
            }
            
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(PlayerIsHit());
                other.gameObject.GetComponent<PlayerController>().TakeDamage(soEnemy.bodyDamage);
            }
        }
    }
}