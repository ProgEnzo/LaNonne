using System.Linq;
using AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class Blade : MonoBehaviour
    {
        public bool isHitting;
        public float hitLength;
        public float hitAngle = 45f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public LineRenderer lineRenderer;
        public BoxCollider2D boxCollider;
        public Quaternion finalRotation1;
        public Quaternion finalRotation2;
        private PlayerController playerController;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private int hitState;
        [SerializeField] private int maxHitState;
        [SerializeField] private float maxComboCooldown;
        private float currentComboCooldown;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
            currentComboCooldown = maxComboCooldown;
            hitState = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            ZealousBlade();
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            lineRenderer.SetPosition(1, new Vector3(0, hitLength/parentLocalScaleX, 0));
            boxCollider.size = new Vector2(0.1f/parentLocalScaleX, hitLength/parentLocalScaleX);
            boxCollider.offset = new Vector2(0, hitLength/parentLocalScaleX/2);
        }

        private void ZealousBlade()
        {
            currentComboCooldown -= Time.deltaTime;
            if (currentComboCooldown <= 0)
            {
                hitState = 0;
            }

            if (Input.GetMouseButtonDown(0) && !isHitting)
            {
                hitState += 1;
                currentComboCooldown = maxComboCooldown;
                
                var newDirection = (playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), playerController.transform.localScale.x) switch
                {
                    (0, > 0 or < 0) => Vector2.down,
                    (1, > 0 or < 0) => Vector2.up,
                    (2, > 0) => Vector2.right,
                    (2, < 0) => Vector2.left,
                    _ => Vector2.right
                };
                var newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                AnimationControllerBool(IsAttacking);
                finalRotation1 = newRotation * Quaternion.Euler(0, 0, hitAngle / 2);
                finalRotation2 = newRotation * Quaternion.Euler(0, 0, -hitAngle / 2);
                lineRenderer.enabled = true;
                boxCollider.enabled = true;
                isHitting = true;

                if (hitState % 2 == 1)
                {
                    transform.rotation = finalRotation2;
                }
                else
                {
                    transform.rotation = finalRotation1;
                }
            }

            if (isHitting)
            {
                if (hitState % 2 == 1)
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation1, hitSpeed * Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation1.eulerAngles.z + toleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation1.eulerAngles.z - toleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        AnimationManagerBool(IsAttacking, false);
                        playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                        if (hitState == maxHitState)
                        {
                            hitState = 0;
                        }
                    }
                }
                else
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation2, hitSpeed * Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation2.eulerAngles.z + toleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation2.eulerAngles.z - toleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                        boxCollider.enabled = false;
                        AnimationManagerBool(IsAttacking, false);
                        playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                        if (hitState == maxHitState)
                        {
                            hitState = 0;
                        }
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //DMG du player sur les enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyController>().TakeDamageFromPlayer(soController.playerAttackDamage);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }
            
            //DMG du player sur le BOSS
            if (other.gameObject.CompareTag("Boss"))
            {
                other.gameObject.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(soController.playerAttackDamage);
            }
        }
        
        private void AnimationControllerBool(int parameterToChange)
        {
            AnimationManagerBool(parameterToChange, true);
            StartCoroutine(playerController.CanChangeCoroutine());
        }
        
        private void AnimationManagerBool(int parameterToChange, bool value)
        {
            if (parameterToChange  == IsAttacking)
            {
                playerController.AnimationManagerSwitch(playerController.currentAnimPrefabAnimator.GetInteger(DirectionState), playerController.currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }

            foreach (var prefab in playerController.animPrefabs.Where(prefab => prefab != playerController.currentAnimPrefab))
            {
                prefab.SetActive(false);
            }
        }
    }
}
