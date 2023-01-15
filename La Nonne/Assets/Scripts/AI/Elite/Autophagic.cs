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

        private void Awake()
        {
            soAutophagic = (SoAutophagic) soEnemy;
            aiPath = GetComponent<AIPath>();
            
            //SOUND MOVEMENT
            autophageAudioSource.clip = autophageMovementAudioClip;
            autophageAudioSource.Play();
            
            totalTime = soAutophagic.maxTimer / soAutophagic.autoDamagePart;
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
    }
}
