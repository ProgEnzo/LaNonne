using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.So;
using Pathfinding;
using UnityEngine;

namespace AI.Elite
{
    public class CareTaker : EnemyController
    {
        internal SoCaretaker soCaretaker;
        
        [Header("Enemy Attack")]
        private float cooldownTimer;
        private bool blinkExecuted;

        [Header("Enemy Components")]
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;

        private List<GameObject> y;

        protected override void Start()
        {
            base.Start();
            soCaretaker = (SoCaretaker) soEnemy;
            GoToTheNearestMob();
            
            //Zones de heal / dégâts
            circle.enabled = false;
            circleSprite.SetActive(false);
        }

        private void CircleTimer()
        {
            //Timer for spawning circles
            if (!blinkExecuted && cooldownTimer <= 0f)
            {
                blinkExecuted = true;
                StartCoroutine(BlinkCircle());
            }
            else
            {
                cooldownTimer -= Time.deltaTime; // cooldowntimer = -1 toutes les secondes
            }
        }

        private IEnumerator BlinkCircle()
        {
            circle.enabled = true;
            circleSprite.SetActive(true);
            currentHealth += soCaretaker.healAmount;
            yield return new WaitForSeconds(soCaretaker.timeBetweenCircleSpawn);
            circle.enabled = false;
            circleSprite.SetActive(false);
            cooldownTimer = soCaretaker.timeBetweenCircleSpawn;
            blinkExecuted = false;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, soCaretaker.attackRange);
        }

        private void GoToTheNearestMob()
        {
            // y = liste de tous les gameobjects mobs/enemy
        
            //tous les gameobjects de la scène vont se mettre dans "y", transforme le dico en liste (parce qu'on peut pas modif les valeurs du dico)
            y = FindObjectsOfType<GameObject>().ToList(); 
        

            foreach (var gObject in y.ToList()) //pour chaque gObject dans y alors exécute le script
            {
                //exécuter ce script pour tous les game objects sauf ces mobs 
                if (!gObject.CompareTag("Enemy") || gObject == gameObject)
                {
                    y.Remove(gObject);
                }
            }
        
            y = y.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();
        
            //prendre l'enemy le plus proche en new target (le premier de la liste)
            if (y[0])
            {
                GetComponent<AIDestinationSetter>().target = y[0].transform;
            }
        }
    
        void CheckIfTargetIsDead()
        {
            if (GetComponent<AIDestinationSetter>().target == null)
            {
                GoToTheNearestMob();
            }
        }
        
        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player"))
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(soCaretaker.bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * soCaretaker.bodyKnockBack;
            
                playerController.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
            }
        }
    }
}
