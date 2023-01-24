using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.So;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable CommentTypo

namespace AI.Elite
{
    public class  PyromaniacProjectile : MonoBehaviour
    {
        private float explosionRadius;
        private float tolerance;
        internal Vector3 destination;
        [SerializeField] private ParticleSystem throwVFX;
        [SerializeField] private ParticleSystem explosionVFX;
        internal bool isExploded;
        internal Coroutine currentCoroutine;
        
        [SerializeField] private SoPyromaniac soPyromaniac;
        
        [Header("SoundEffect")]
        public AudioSource pyroAudioSource;
        public AudioClip[] pyroProjectileExplosionAudioClip;
        private Coroutine soundExplosionProjectile;

        private void OnEnable()
        {
            //PLAY PARTICULE LANCER DE FLAMME
            throwVFX.Play(); //On réactive le sprite du projectile

            //STOP PARTICULE EXPLOSION
            explosionVFX.Stop();
            
            explosionRadius = soPyromaniac.explosionRadiusIndicator / 2; //On divise par 2 car le cercle est trop grand
            tolerance = soPyromaniac.explosionTolerance;
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
                
                //SOUND PROJECTILE EXPLOSION
                pyroAudioSource.PlayOneShot(pyroProjectileExplosionAudioClip[Random.Range(0, pyroProjectileExplosionAudioClip.Length)]);
                
                //Arrêt de la vitesse
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                
                //Cercle d'explosion
                explosionVFX.Play();
                
                //Explosion
                var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
                Physics2D.CircleCast(transform.position, explosionRadius*transform.parent.localScale.x*transform.localScale.x, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
                if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
                {
                    foreach (var playerGameObject in objectsInArea.Select(hit => hit.collider.gameObject).Where(playerGameObject => playerGameObject.CompareTag("Player")))
                    {
                        playerRef.TakeDamage(soPyromaniac.bodyDamage); //Le joueur prend des dégâts

                        var direction = (playerGameObject.transform.position - transform.position).normalized;
                        var knockBack = direction * soPyromaniac.knockBackPower;

                        playerRef.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
                    }
                }

                yield return new WaitForSeconds(0.1f);
                throwVFX.Stop(); //On désactive le sprite du projectile
                yield return new WaitForSeconds(soPyromaniac.fireCooldown);
                currentCoroutine = null;
            }
        }

        private IEnumerator BlinkFire()
        {
            //Dégâts de feu
            var playerRef = PlayerController.instance;
            var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
            Physics2D.CircleCast(transform.position, explosionRadius*transform.parent.localScale.x*transform.localScale.x * 1.8f, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
            if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
            {
                foreach (var unused in objectsInArea.Where(hit => hit.collider.CompareTag("Player")))
                {
                    playerRef.TakeDamage(soPyromaniac.bodyDamage); //Le joueur prend des dégâts
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(soPyromaniac.fireCooldown);
            currentCoroutine = null;
        }
    }
}