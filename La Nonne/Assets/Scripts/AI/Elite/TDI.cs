using System.Collections;
using Controller;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace AI.Elite
{
    public class TDI : EnemyController
    {
        [Header("Enemy Attack")] 
        [SerializeField] public int bodyDamage;
        [SerializeField] public int bodyKnockBack;
        [SerializeField] internal int circleDamage;
        [SerializeField] public bool blinkExecuted;
        
        
        [HideInInspector] public float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;
        
        [SerializeField] internal float healAmount;
        
        [Header("Components")] 
        [SerializeField] public GameObject bully;
        [SerializeField] public GameObject caretaker;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;
        
        protected override void Start()
        {
            base.Start();
            cooldownTimer = timeBetweenCircleSpawn;
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
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(bodyDamage); //Player takes damage

                var colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                var knockBack = direction * bodyKnockBack;
            
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
            circle.enabled = true;
            circleSprite.SetActive(true);
            currentHealth += healAmount;
            yield return new WaitForSeconds(timeBetweenCircleSpawn);
            circle.enabled = false;
            circleSprite.SetActive(false);
            cooldownTimer = timeBetweenCircleSpawn;
            blinkExecuted = false;
        }
    }
}
