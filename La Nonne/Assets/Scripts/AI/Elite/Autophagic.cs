using System.Collections;
using AI.So;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Elite
{
    public class Autophagic : EnemyController
    {
        private float currentTimer;
        private bool isActivated;
        private readonly int[] positionMultiplierArray = {-1, 1};
        private AIPath aiPath;
        private int stunBarCurrentDamages;
        private SoAutophagic soAutophagic;

        private void Awake()
        {
            soAutophagic = (SoAutophagic) soEnemy;
            aiPath = GetComponent<AIPath>();
            stunBarCurrentDamages = 0;
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
            }
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
            EnemyDeath();
        }
        
        public override void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;
            EnemyDeath();
            stunBarCurrentDamages += damage;
            if (stunBarCurrentDamages < soAutophagic.stunBarMaxDamages) return;
            stunBarCurrentDamages = 0;
            StartCoroutine(FullStunBar());
        }
        
        private IEnumerator FullStunBar()
        {
            isStunned = true;
            yield return new WaitForSeconds(soAutophagic.stunBarDuration);
            isStunned = false;
        }
    }
}
