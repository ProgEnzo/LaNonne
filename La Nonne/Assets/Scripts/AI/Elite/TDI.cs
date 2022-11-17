using System.Collections;
using AI.Trash;
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
        [SerializeField] public int bodyKnockback;
        [SerializeField] public int circleDamage;
        [SerializeField] public bool blinkExecuted;
        
        
        [HideInInspector] public float cooldownTimer;
        [SerializeField] private float timeBetweenCircleSpawn;
        
        [SerializeField] public float healAmount;
        
        [Header("Components")] 
        [SerializeField] public GameObject bully;
        [SerializeField] public GameObject caretaker;
        [SerializeField] public GameObject player;
        [SerializeField] private CircleCollider2D circle;
        [SerializeField] private GameObject circleSprite;
        [HideInInspector] public PlayerController playerController;
        
        protected override void Start()
        {
            base.Start();
            cooldownTimer = timeBetweenCircleSpawn;
        }

        protected override void Update()
        {
            base.Update();
            StartCoroutine(EnemyDeath());

            CircleTimer();
        }

        private void Awake()
        {
            //Assignation du script playerController au prefab ON SPAWN
            playerController = PlayerController.instance;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            //Heal le TrashMobCLose
            if (col.gameObject.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<EnemyController>().currentHealth += healAmount;
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }

            if (col.gameObject.CompareTag("Player"))
            {
                col.GetComponent<PlayerController>().TakeDamage(circleDamage); //Player takes damage
                StartCoroutine(PlayerIsHit());
            }
        }

        private void OnCollisionEnter2D(Collision2D col) 
        {
            //Si le bully touche le player
            if (col.gameObject.CompareTag("Player"))
            {
                StartCoroutine(PlayerIsHit());
                playerController.TakeDamage(bodyDamage); //Player takes damage

                Collider2D colCollider = col.collider; //the incoming collider2D (celle du player en l'occurence)
                Vector2 direction = (colCollider.transform.position - transform.position).normalized;
                Vector2 knockback = direction * bodyKnockback;
            
                playerController.mRigidbody.AddForce(knockback, ForceMode2D.Impulse);
            }
        }

        #region HealthEnemy
        private new IEnumerator EnemyDeath()
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
