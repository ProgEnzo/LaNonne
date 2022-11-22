using System.Collections;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Elite
{
    public class Autophagic : EnemyController
    {
        [SerializeField] private float detectionRadius;
        [SerializeField] private float autoDamagePart;
        [SerializeField] private float maxTimer;
        private float currentTimer;
        private bool isActivated;
        [SerializeField] private float avoidedAngle;
        [SerializeField] private float destinationMinRangeValue;
        [SerializeField] private float destinationMaxRangeValue;
        private readonly int[] positionMultiplierArray = new int[2];
        private AIPath aiPath;
        [SerializeField] private int stunBarMaxDamages;
        private int stunBarCurrentDamages;
        [SerializeField] private float stunBarDuration;

        private void Awake()
        {
            positionMultiplierArray[0] = -1;
            positionMultiplierArray[1] = 1;
            aiPath = GetComponent<AIPath>();
            stunBarCurrentDamages = 0;
        }

        protected override void Update()
        {
            base.Update();
            
            if (!isActivated)
            {
                var distanceToPlayer = Vector2.Distance(playerController.transform.position, transform.position);
                if (distanceToPlayer < detectionRadius)
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

                    currentTimer = maxTimer;
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
                    transform.position.x + Random.Range(destinationMinRangeValue, destinationMaxRangeValue) *
                    positionMultiplierArray[Random.Range(0, 2)],
                    transform.position.y + Random.Range(destinationMinRangeValue, destinationMaxRangeValue) *
                    positionMultiplierArray[Random.Range(0, 2)]);
                var newRotationLook = Quaternion.LookRotation(Vector3.forward, destination - (Vector2)transform.position);
                isNewDirectionOk = (newRotationLook.eulerAngles.z - rotationLook.eulerAngles.z) switch
                {
                    var angle and >= 0 when angle < avoidedAngle / 2 => false,
                    var angle and < 0 when angle > -avoidedAngle / 2 => false,
                    _ => true
                };
            } while (!isNewDirectionOk);

            aiPath.destination = destination;
        }

        private void TakeAutoDamage()
        {
            currentHealth -= (int)(soEnemy.maxHealth * autoDamagePart);
            EnemyDeath();
        }
        
        public override void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;
            EnemyDeath();
            stunBarCurrentDamages += damage;
            if (stunBarCurrentDamages < stunBarMaxDamages) return;
            stunBarCurrentDamages = 0;
            StartCoroutine(FullStunBar());
        }
        
        private IEnumerator FullStunBar()
        {
            isStunned = true;
            yield return new WaitForSeconds(stunBarDuration);
            isStunned = false;
        }
    }
}
