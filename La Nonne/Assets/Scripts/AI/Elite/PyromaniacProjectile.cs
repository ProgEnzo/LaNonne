using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable CommentTypo

namespace AI.Elite
{
    public class PyromaniacProjectile : MonoBehaviour
    {
        [SerializeField] public float explosionRadiusIndicator = 5f;
        private float explosionRadius;
        public float tolerance;
        public Vector3 destination;
        private GameObject circleGameObject;
        public bool isExploded;
        [SerializeField] private float knockBackPower;
        private Coroutine currentCoroutine;
        [SerializeField] private float fireCooldown = 1f;
        
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        private void OnEnable()
        {
            GetComponent<SpriteRenderer>().enabled = true; //On réactive le sprite du projectile
            circleGameObject = transform.GetChild(0).gameObject; //Initialisation de l'accès au cercle
            circleGameObject.SetActive(false); //On le désactive pour le moment
            explosionRadius = explosionRadiusIndicator / 2; //On divise par 2 car le cercle est trop grand
        }

        private void Update()
        {
            if (!isExploded)
            {
                currentCoroutine ??= StartCoroutine(StopVelocity(destination));
            }
            else
            {
                currentCoroutine ??= StartCoroutine(BlinkFire());
            }
        }

        private IEnumerator StopVelocity(Vector3 position)
        {
            if (position.x-tolerance < transform.position.x && transform.position.x < position.x+tolerance && position.y-tolerance < transform.position.y && transform.position.y < position.y+tolerance)
            {
                var playerRef = PlayerController.instance;
                isExploded = true;
                
                //Arrêt de la vitesse
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                
                //Cercle d'explosion
                circleGameObject.SetActive(true); //On active le cercle
                circleGameObject.transform.localScale = Vector3.one * (explosionRadius * 8); //On le met à la bonne taille
                var circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
                var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f); //Opacité du cercle
                
                //Explosion
                var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
                Physics2D.CircleCast(transform.position, explosionRadius, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
                if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
                {
                    foreach (var playerGameObject in objectsInArea.Select(hit => hit.collider.gameObject).Where(playerGameObject => playerGameObject.CompareTag("Player")))
                    {
                        StartCoroutine(PlayerIsHit());
                        playerRef.TakeDamage(soEnemy.bodyDamage); //Le joueur prend des dégâts

                        var direction = (playerGameObject.transform.position - transform.position).normalized;
                        var knockBack = direction * knockBackPower;

                        playerRef.m_rigidbody.AddForce(knockBack, ForceMode2D.Impulse);
                    }
                }

                yield return new WaitForSeconds(0.1f);
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f); //Transparence du cercle
                GetComponent<SpriteRenderer>().enabled = false; //On désactive le sprite du projectile
                yield return new WaitForSeconds(fireCooldown);
                currentCoroutine = null;
            }
        }

        private static IEnumerator PlayerIsHit()
        {
            var playerRef = PlayerController.instance;
            playerRef.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRef.GetComponent<SpriteRenderer>().color = Color.yellow;
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
            currentCoroutine = null;
        }
    }
}