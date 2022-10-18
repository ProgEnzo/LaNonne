using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace AI.Elite
{
    public class PyromaneProjectile : MonoBehaviour
    {
        public float explosionRadius = 5f;
        public float tolerance;
        public Vector3 destination;
        private GameObject circleGameObject;
        
        void OnEnable()
        {
            circleGameObject = transform.GetChild(0).gameObject;
            SpriteRenderer circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>();
            var color = circleSpriteRenderer.color;
            circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f);
        }

        // Update is called once per frame
        void Update()
        {
            StartCoroutine(StopVelocity(destination));
            PlayerController playerRef = PlayerController.instance;
            circleGameObject.transform.position = destination;
            circleGameObject.transform.localScale = Vector3.one * explosionRadius;
        }

        public IEnumerator StopVelocity(Vector3 position)
        {
            if (position.x-tolerance < transform.position.x && transform.position.x < position.x+tolerance && position.y-tolerance < transform.position.y && transform.position.y < position.y+tolerance)
            {
                PlayerController playerRef = PlayerController.instance;
                SpriteRenderer circleSpriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                circleSpriteRenderer.color = new Color(circleSpriteRenderer.color.r, circleSpriteRenderer.color.g, circleSpriteRenderer.color.b, 0.5f);
                List<RaycastHit2D> ennemiesInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(transform.position, explosionRadius, Vector2.zero, new ContactFilter2D(), ennemiesInArea);
                if (ennemiesInArea != new List<RaycastHit2D>())
                {
                    foreach (RaycastHit2D hit in ennemiesInArea)
                    {
                        /*if (hit.collider.CompareTag("Ennemy"))
                        {
                            playerRef.MakeDamage(hit.collider.gameObject, so.baseDmg);
                            Vector3 dir = hit.transform.position - playerRef.transform.position;
                            Vector3 dirNormalized = dir.normalized;
                            hit.transform.position = new Vector2(hit.transform.position.x + dirNormalized.x * so.knockBack, hit.transform.position.y + dirNormalized.y * so.knockBack);
                        }*/
                    }
                }
                yield return new WaitForSeconds(0.1f);
                gameObject.SetActive(false);
            }
        }
    }
}