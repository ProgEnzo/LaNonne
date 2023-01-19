using System;
using AI.So;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AI.Elite
{
    public class Autophagic : EnemyController
    {
        private float currentTimer;
        private float currentTotalTimer;
        private float totalTime;
        private bool isActivated;
        private readonly int[] positionMultiplierArray = {-1, 1};
        private AIPath aiPath;
        private SoAutophagic soAutophagic;
        [SerializeField] private Image epSprite;

        [Header("SoundEffect")]
        public AudioSource autophageAudioSource;
        public AudioClip autophageMovementAudioClip;
        public AudioClip[] autophageEatingAudioClip;

        private void Awake()
        {
            soAutophagic = (SoAutophagic) soEnemy;
            aiPath = GetComponent<AIPath>();
            
            //SOUND MOVEMENT
            autophageAudioSource.clip = autophageMovementAudioClip;
            autophageAudioSource.Play();
            
            totalTime = soAutophagic.maxTimer / soAutophagic.autoDamagePart;
            currentTimer = soAutophagic.maxTimer;
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!isActivated)
            {
                var distanceToPlayer = Vector2.Distance(playerController.transform.position, transform.position);
                if (distanceToPlayer < soAutophagic.detectionRadius)
                {
                    isActivated = true;
                }
            }
            else
            {
                if (currentTimer <= 0)
                {
                    TakeAutoDamage();
                    
                    ChangeDestination();

                    currentTimer = soAutophagic.maxTimer;
                }
                currentTimer -= Time.deltaTime;
                
                currentTotalTimer += Time.deltaTime;
                epSprite.fillAmount = (totalTime - currentTotalTimer) / totalTime;
            }
            
            CheckDirection();
        }

        private void ChangeDestination()
        {
            var newDirection = playerController.transform.position - transform.position;
            newDirection.z = 0;
            newDirection.Normalize();
            var rotationLook = Quaternion.LookRotation(Vector3.forward, newDirection);
            bool isNewDirectionOk;
            Vector2 destination;

            do
            {
                destination = new Vector2(
                    transform.position.x + Random.Range(soAutophagic.destinationMinRangeValue, soAutophagic.destinationMaxRangeValue) *
                    positionMultiplierArray[Random.Range(0, 2)],
                    transform.position.y + Random.Range(soAutophagic.destinationMinRangeValue, soAutophagic.destinationMaxRangeValue) *
                    positionMultiplierArray[Random.Range(0, 2)]);
                var newRotationLook = Quaternion.LookRotation(Vector3.forward, destination - (Vector2)transform.position);
                isNewDirectionOk = (newRotationLook.eulerAngles.z - rotationLook.eulerAngles.z) switch
                {
                    var angle and >= 0 when angle < soAutophagic.avoidedAngle / 2 => false,
                    var angle and < 0 when angle > -soAutophagic.avoidedAngle / 2 => false,
                    _ => true
                };
            } while (!isNewDirectionOk);

            aiPath.destination = destination;
        }

        private void TakeAutoDamage()
        {
            currentHealth -= (int)(soAutophagic.maxHealth * soAutophagic.autoDamagePart);
            autophageAudioSource.PlayOneShot(autophageEatingAudioClip[Random.Range(0, autophageEatingAudioClip.Length)]);
            
            EnemyDeath();
        }
        
        protected override void EnemyDeath()
        {
            if (currentHealth <= 0)
            {
                EpDrop((int)(soEnemy.numberOfEp * currentEpDropMultiplier * (totalTime - currentTotalTimer) / totalTime));
                
                scoreManager.AddKilledEnemyScore(soEnemy.scorePoint);

                Destroy(gameObject); //Dies
            }
        }
        
        private void CheckDirection()
        {
            var puppetLocalScale = enemyPuppet.transform.localScale;
            if (rb.velocity.x != 0)
            {
                enemyPuppet.transform.localScale = new Vector3(MathF.Sign(rb.velocity.x) * MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
            else
            {
                enemyPuppet.transform.localScale = playerController.transform.position.x > transform.position.x ? new Vector3(MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z) : new Vector3(-MathF.Abs(puppetLocalScale.x), puppetLocalScale.y, puppetLocalScale.z);
            }
        }
    }
}
