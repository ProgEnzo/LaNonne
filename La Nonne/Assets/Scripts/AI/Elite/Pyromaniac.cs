using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.So;
using Controller;
using Pathfinding;
using UnityEngine;
// ReSharper disable CommentTypo

namespace AI.Elite
{
    public class Pyromaniac : EnemyController
    {
        private SoPyromaniac soPyromaniac;
        [SerializeField] private GameObject circleGameObject;
        private Vector2 dashDirection;
        private float currentFireTrailMaxLength;
        private Vector3 boxCastOrigin;
        private Vector3 boxCastDestination;
        private Vector3 dashInitialPosition;
        private Coroutine currentCoroutine;
        private Coroutine currentHittingCoroutine;
        private AIDestinationSetter aiDestinationSetter;
        private AIPath scriptAIPath;
        private bool scriptAIPathState;
        private bool isDashing;
        private bool delayedIsDashing;
        private bool isProjectileOn;
        private bool isImpactOn;
        private bool canBoxCast;
        [SerializeField] private PyromaniacProjectile projectile;
        private CapsuleCollider2D capsuleCollider2D;
        private LineRenderer lineRenderer;

        [Header("SoundEffect")]
        public AudioSource pyroAudioSource;
        public AudioClip pyroImpactAudioClip;
        public AudioClip pyroLaunchProjectileAudioClip;
        public AudioClip[] pyroFireTrailAudioClip;
        private Coroutine soundLaunchProjectile;
        private Coroutine soundFireTrail;
        
        protected override void Start()
        {
            base.Start();
            
            //Initialoisation du scriptable object
            soPyromaniac = (SoPyromaniac) soEnemy;
            
            //Initialisation des components
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            lineRenderer = GetComponent<LineRenderer>();
            
            //Initialisation du déplacement
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            scriptAIPath = GetComponent<AIPath>();
            scriptAIPathState = true;
            
            //Initialisation des états du pyromane
            isDashing = false; //Dash
            delayedIsDashing = false; //Dash
            isProjectileOn = false; //Existence du projectile
            isImpactOn = false; //Impact
            canBoxCast = false; //Zone de feu

            //Initialisation de la vitesse de dash
            currentVelocitySpeed = 0f;
            
            //Initialisation du line renderer
            lineRenderer.enabled = false;
            lineRenderer.widthMultiplier = transform.localScale.x;
        }

        protected override void Update()
        {
            base.Update();
            
            //Pour Chill
            if (!isKnockedBack)
            {
                rb.velocity = dashDirection * currentVelocitySpeed;
            }
            
            //Initialisation de variables locales pour l'optimisation
            var playerPosition = playerController.transform.position; //Position du joueur
            var transform1 = transform;
            var position = transform1.position; //Position du pyromane
            var projectilePosition = projectile.transform.position; //Position du projectile
            
            //Tant que le projectile n'a pas explosé
            if (!projectile.isExploded)
            {
                //Seulement si le projectile n'existe pas, et que le pyromane n'est dans l'état ni de dash ni d'impact
                if (!isProjectileOn && !isDashing && !isImpactOn)
                {
                    //Si le joueur est dans le rayon de détection, on active le projectile
                    if (Vector3.Distance(position, playerPosition) <= soPyromaniac.detectionRadius)
                    {
                        //Initialisation des variables pour l'état de lancer
                        isProjectileOn = true; //Le projectile existe
                        aiDestinationSetter.enabled = false; //Le pyromane ne se déplace plus vers le joueur
                        scriptAIPath.maxSpeed = 0f; //Le pyromane ne se déplace plus avec A*
                        scriptAIPathState = false; //Le pyromane ne se déplace plus avec A*
                        canBoxCast = false; //La zone de feu du pyromane est désactivée
                        lineRenderer.enabled = false; //Le line renderer est désactivé
                        
                        //Stop sound trail
                        pyroAudioSource.Stop();
                        
                        //Lancement du projectile
                        //sound lancement projectile
                        pyroAudioSource.PlayOneShot(pyroLaunchProjectileAudioClip);
                        
                        var distanceToCross = Mathf.Min(Vector3.Distance(position, playerPosition), soPyromaniac.throwRadius); //On calcule la distance à parcourir par le projectile. On prend la distance entre le joueur et le pyromane, et on la limite à la distance maximale de lancer du projectile.
                        var newPositionVector = (playerPosition - position).normalized * distanceToCross; //On calcule le vecteur de déplacement du projectile
                        var newPosition = position + newPositionVector; //On calcule la nouvelle position du projectile
                        ThrowProjectile(newPosition, newPositionVector.normalized); //On lance le projectile à la nouvelle position, avec la nouvelle direction
                    }
                    //Sinon, le pyromane se déplace vers le joueur avec A*
                    else
                    {
                        //Déplacement du pyromane
                        aiDestinationSetter.enabled = true; //Le pyromane se déplace vers le joueur
                        scriptAIPathState = true; //Le pyromane se déplace avec A*
                        scriptAIPath.maxSpeed = 3f; //Le pyromane se déplace à la vitesse normale
                    }
                }
            }
            //Une fois que le projectile a explosé
            else
            {
                //Si le pyromane n'est dans l'état ni de dash ni d'impact
                if (!isDashing && !isImpactOn)
                {
                    //Le projectile n'existe plus
                    isProjectileOn = false;
                    
                    //Dash vers la zone de feu
                    currentCoroutine ??= StartCoroutine(DashToFireZone());
                }
                
                //Si le pyromane passe le projectile
                if (position.x < projectilePosition.x + soPyromaniac.fireTrailTolerance && position.x > projectilePosition.x - soPyromaniac.fireTrailTolerance && position.y < projectilePosition.y + soPyromaniac.fireTrailTolerance && position.y > projectilePosition.y - soPyromaniac.fireTrailTolerance)
                {
                    //La zone de feu est activée
                    canBoxCast = true;
                    lineRenderer.enabled = true;
                    
                    //Play sound trail
                    pyroAudioSource.clip = pyroFireTrailAudioClip[Random.Range(0, pyroFireTrailAudioClip.Length)];
                    pyroAudioSource.Play();

                }
            }
            
            //Si la zone de feu du pyromane est activée
            if (canBoxCast)
            {
                //Les conditions ne sont pas bonnes, à corriger
                if (!projectile.isExploded && !isProjectileOn && !isDashing && !isImpactOn)
                {
                    currentFireTrailMaxLength -= Time.deltaTime * soPyromaniac.fireTrailDisappearanceSpeed;

                    if (currentFireTrailMaxLength <= soPyromaniac.fireTrailTolerance)
                    {
                        canBoxCast = false;
                        lineRenderer.enabled = false;
                        
                        //Stop sound trail
                        pyroAudioSource.Stop();
                    }
                }
                else
                {
                    boxCastOrigin = position;
                    boxCastDestination = projectilePosition;
                    currentFireTrailMaxLength = Vector2.Distance(boxCastDestination, boxCastOrigin);
                }
                
                var objectsInArea = new List<RaycastHit2D>();
                Physics2D.CircleCast(boxCastOrigin, transform1.localScale.x * 0.5f, (boxCastDestination - boxCastOrigin).normalized, new ContactFilter2D(), objectsInArea, currentFireTrailMaxLength);
                foreach (var unused in objectsInArea.Where(hit => hit.collider.CompareTag("Player")))
                {
                    currentHittingCoroutine ??= StartCoroutine(FireDamage());
                }
                //BoxCastDebug.DrawBoxCast2D(boxCastOrigin, Vector2.one * transform1.localScale * 2, 0f, (boxCastDestination - boxCastOrigin).normalized, currentFireTrailMaxLength, Color.magenta);
                lineRenderer.SetPosition(0, Vector2.zero);
                lineRenderer.SetPosition(1, (boxCastDestination - boxCastOrigin).normalized * currentFireTrailMaxLength);
            }
        }

        private void ThrowProjectile(Vector3 destination, Vector3 direction)
        {
            var transform1 = transform;
            projectile.transform.position = transform1.position;
            projectile.gameObject.SetActive(true);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * soPyromaniac.projectileSpeed;
            projectile.destination = destination;
        }
        
        private IEnumerator DashToFireZone()
        {
            yield return new WaitForSeconds(0.5f);
            var transform1 = transform;
            dashInitialPosition = transform1.position; //Position du pyromane
            isDashing = true;
            StartCoroutine(DelayedIsDashing());
            capsuleCollider2D.enabled = false;
            dashDirection = (projectile.transform.position - dashInitialPosition).normalized;
            currentVelocitySpeed = soEnemy.velocityBasicSpeed;
        }
        
        private IEnumerator FireDamage()
        {
            playerController.TakeDamage(soEnemy.bodyDamage);
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
            var objectsInArea = new List<RaycastHit2D>(); //Déclaration de la liste des objets dans la zone d'explosion
            Physics2D.CircleCast(transform.position, soPyromaniac.explosionRadius, Vector2.zero, new ContactFilter2D(), objectsInArea); //On récupère les objets dans la zone d'explosion
            if (objectsInArea != new List<RaycastHit2D>()) //Si la liste n'est pas vide
            {
                foreach (var unused in objectsInArea.Where(hit => hit.collider.CompareTag("Player")))
                {
                    playerController.TakeDamage(soEnemy.bodyDamage); //Le joueur prend des dégâts
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.25f); //Transparence du cercle
            yield return new WaitForSeconds(soPyromaniac.fireCooldown);
            
            circleGameObject.SetActive(false); //On désactive le cercle
            isImpactOn = false;
            var transform1 = transform;
            projectile.transform.position = transform1.position;
            projectile.isExploded = false;
            projectile.currentCoroutine = null;
            projectile.gameObject.SetActive(false);
            
            currentCoroutine = null;
        }

        protected override void StunCheck()
        {
            if (scriptAIPathState)
            {
                base.StunCheck();
            }
            else
            {
                scriptAIPath.enabled = false;
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, soPyromaniac.detectionRadius);
            Gizmos.DrawWireSphere(position, soPyromaniac.throwRadius);
            if (canBoxCast)
            {
                Gizmos.DrawWireSphere(boxCastOrigin, transform.localScale.x * 0.5f);
                Gizmos.DrawWireSphere(boxCastOrigin + (boxCastDestination - boxCastOrigin).normalized * currentFireTrailMaxLength, transform.localScale.x * 0.5f);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Impact(isDashing);
            
            pyroAudioSource.PlayOneShot(pyroImpactAudioClip);

            
            if (other.gameObject.CompareTag("Player") && !playerController.isRevealingDashOn && !isStunned)
            {
                other.gameObject.GetComponent<PlayerController>().TakeDamage(soEnemy.bodyDamage);
            }
        }

        private void OnCollisionStay2D()
        {
            Impact(delayedIsDashing);
        }

        private void Impact(bool isDash)
        {
            if (isDash && !isImpactOn)
            {
                currentVelocitySpeed = 0f;
                isDashing = false;
                delayedIsDashing = false;
                capsuleCollider2D.enabled = true;
                isImpactOn = true;
                
                //Cercle d'explosion
                circleGameObject.SetActive(true); //On active le cercle
                circleGameObject.transform.localScale = Vector3.one * (soPyromaniac.explosionRadius * 8); //On le met à la bonne taille
                var circleSpriteRenderer = circleGameObject.GetComponent<SpriteRenderer>(); //Accès au sprite renderer du cercle
                var color = circleSpriteRenderer.color; //Accès à la couleur du cercle
                circleSpriteRenderer.color = new Color(color.r, color.g, color.b, 0.5f); //Opacité du cercle

                StartCoroutine(BlinkFire());
            }
        }
        
        private IEnumerator DelayedIsDashing()
        {
            yield return new WaitForSeconds(0.1f);
            delayedIsDashing = true;
        }

        internal override void HitStopAndKnockBack(float hitStopDuration, float knockBackForce)
        {
            isKnockedBack = true;
            rb.AddForce((transform.position - playerController.transform.position).normalized * knockBackForce, ForceMode2D.Impulse);
        }

        internal override void HitStop(float hitStopDuration) { }
    }
}