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
            /*var circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
            var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
            circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f); //Initialisation de la transparence du cercle*/
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
                /*SpriteRenderer circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>();
                var color = circleSpriteRenderer.color;
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f);*/
                var objectsInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, explosionRadius, Vector2.zero, new ContactFilter2D(), objectsInArea);
                if (objectsInArea != new List<RaycastHit2D>())
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
                        
                        /*Vector3 dir = hit.transform.position - playerRef.transform.position;
                        Vector3 dirNormalized = dir.normalized;
                        hit.transform.position = new Vector2(hit.transform.position.x + dirNormalized.x * so.knockBack, hit.transform.position.y + dirNormalized.y * so.knockBack);*/
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