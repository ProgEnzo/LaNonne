using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable CommentTypo

namespace AI.Elite
{
    public class PyromaniacProjectile : MonoBehaviour
    {
        public float explosionRadius = 5f;
        public float tolerance;
        public Vector3 destination;
        private GameObject circleGameObject;
        private bool isExploded;
        [SerializeField] private float knockBackPower;
        
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        private void OnEnable()
        {
            circleGameObject = transform.GetChild(0).gameObject; //Initialisation de l'accès au cercle
            circleGameObject.SetActive(false); //On le désactive pour le moment
        }

        private void Update()
        {
            if (!isExploded)
            {
                StartCoroutine(StopVelocity(destination));
            }
            else
            {
                circleGameObject.transform.position = destination;
                circleGameObject.transform.localScale = Vector3.one * explosionRadius;
            }
        }

        private IEnumerator StopVelocity(Vector3 position)
        {
            if (position.x-tolerance < transform.position.x && transform.position.x < position.x+tolerance && position.y-tolerance < transform.position.y && transform.position.y < position.y+tolerance)
            {
                var playerRef = PlayerController.instance;
                circleGameObject.SetActive(true); //On active le cercle
                SpriteRenderer circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
                var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f); //Opacité du cercle
                var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
                Physics2D.CircleCast(transform.position, explosionRadius, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
                if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
                {
                    foreach (var hit in objectsInArea)
                    {
                        var playerGameObject = hit.collider.gameObject; //Accès au gameobject du joueur
                        
                        //Si le projectile touche le joueur
                        if (playerGameObject.CompareTag("Player"))
                        {
                            StartCoroutine(PlayerIsHit());
                            playerRef.TakeDamage(soEnemy.bodyDamage); //Player takes damage

                            Vector2 direction = (playerGameObject.transform.position - transform.position).normalized;
                            Vector2 knockBack = direction * knockBackPower;

                            playerRef.m_rigidbody.AddForce(knockBack, ForceMode2D.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(0.1f);
                gameObject.SetActive(false);
            }
        }

        private static IEnumerator PlayerIsHit()
        {
            var playerRef = PlayerController.instance;
            playerRef.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRef.GetComponent<SpriteRenderer>().color = Color.yellow;

        }
    }
}