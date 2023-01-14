using System.Collections;
using AI.So;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace AI.Elite
{
    public class TDI : EnemyController
    {
        [Header("Enemy Attack")]
        private bool blinkExecuted;
        private float cooldownTimer;
        
        [Header("Components")]
        [SerializeField] public GameObject bully;
        [SerializeField] public GameObject caretaker;
        [SerializeField] private GameObject circle;
        [SerializeField] private GameObject circleSpriteWarning;
        [SerializeField] private GameObject greenCircleWarning;
        [SerializeField] private ParticleSystem particleHeal;
        [SerializeField] private ParticleSystem particleHeal2;
        internal SoTdi soTdi;
        
        protected override void Start()
        {
            base.Start();
            soTdi = (SoTdi) soEnemy;
            cooldownTimer = soTdi.timeBetweenCircleSpawn;
            
            particleHeal.Stop();
            particleHeal2.Stop();
            
            greenCircleWarning.SetActive(false);
            circle.SetActive(false);


        }

        protected override void Update()
        {
            base.Update();

            CircleTimer();
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player"))
            {
                playerController.TakeDamage(soTdi.bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * soTdi.bodyKnockBack;
            
                playerController.mRigidbody.AddForce(knockBack, ForceMode2D.Impulse);
            }
        }

        #region HealthEnemy
        public override void TakeDamageFromPlayer(int damage)
        {
            currentHealth -= damage;
            StartCoroutine(Split());
        }
        
        private IEnumerator Split()
        {
            if (!(currentHealth <= 50)) yield break;
            transform.DOScale(new Vector3(3, 0, 3), 0.1f);
            scoreManager.AddKilledEnemyScore(soTdi.scorePoint);
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
            var position = transform.position;
            Instantiate(bully, position, quaternion.identity);
            Instantiate(caretaker, position, quaternion.identity);
        }
        #endregion

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
            circleSpriteWarning.SetActive(true);
            greenCircleWarning.SetActive(true);

            circleSpriteWarning.transform.DOScale(new Vector3(0.55f, 0.55f, 0.55f), 0.5f);
            yield return new WaitForSeconds(0.8f);

            greenCircleWarning.SetActive(false);
            circleSpriteWarning.transform.DOScale(new Vector3(0f, 0f, 0f), 0f);

            circleSpriteWarning.SetActive(false);
            circle.SetActive(true);

            particleHeal.Play();
            particleHeal2.Play();
            
            currentHealth += soTdi.healAmount;
            yield return new WaitForSeconds(soTdi.timeBetweenCircleSpawn);
            
            circle.SetActive(false);

            
            particleHeal.Stop();
            particleHeal2.Stop();
            
            cooldownTimer = soTdi.timeBetweenCircleSpawn;
            blinkExecuted = false;
        }
    }
}
