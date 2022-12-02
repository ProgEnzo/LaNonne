using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Pathfinding;
using UnityEngine;

namespace AI.Elite
{
    public class CareTaker : EnemyController
    {
        [Header("Enemy Attack")]
        [SerializeField] internal int circleDamage;
        [SerializeField] private int bodyDamage;
        [SerializeField] internal int healAmount;
        [SerializeField] private float bodyKnockBack;
        [SerializeField] private float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;
        [SerializeField] private float attackRange;
        [SerializeField] public bool blinkExecuted;

        [Header("Enemy Components")]
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;

        public List<GameObject> y;

        protected override void Start()
        {
            base.Start();
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
            currentHealth += healAmount;
            yield return new WaitForSeconds(timeBetweenCircleSpawn);
            circle.enabled = false;
            circleSprite.SetActive(false);
            cooldownTimer = timeBetweenCircleSpawn;
            blinkExecuted = false;
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, attackRange);
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
                playerController.TakeDamage(bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * bodyKnockBack;
            
                playerController.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
            }
        }
    }
}
