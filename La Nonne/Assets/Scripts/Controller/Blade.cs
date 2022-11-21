using System.Collections;
using System.Linq;
using AI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class Blade : MonoBehaviour
    {
        public bool isHitting;
        public bool hitStep;
        public float hitLength;
        public float hitAngle = 45f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public LineRenderer lineRenderer;
        public BoxCollider2D boxCollider;
        public Quaternion finalRotation1;
        public Quaternion finalRotation2;
        private Camera camera1;
        private PlayerController playerController;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private int attackDirectionState;
        private float playerScale;
        private float localStateMult;

        public ChainBladeDamage chainBladeDamage;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            camera1 = Camera.main;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
            playerScale = playerController.transform.localScale.x;
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
            if (Input.GetMouseButtonDown(0) && !isHitting)
            {
                var newDirection = camera1.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                var newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                attackDirectionState = newRotation.eulerAngles.z switch
                {
                    >= 45 and <= 135 => 2,
                    >= 225 and <= 315 => 2,
                    >= 0 and < 45 or > 315 and < 360 => 1,
                    > 135 and < 225 => 0,
                    _ => 0
                };
                localStateMult = newRotation.eulerAngles.z switch
                {
                    >= 45 and <= 135 => -1,
                    >= 225 and <= 315 => 1,
                    >= 0 and < 45 or > 315 and < 360 => 1,
                    > 135 and < 225 => 1,
                    _ => 1
                };
                playerController.transform.localScale = new Vector3(playerScale * localStateMult, playerController.transform.localScale.y, playerController.transform.localScale.z);
                playerController.transform.GetChild(0).localScale = new Vector3(1,  1 * localStateMult, 1);
                AnimationControllerBool(attackDirectionState, IsAttacking);
                finalRotation1 = newRotation * Quaternion.Euler(0, 0, hitAngle / 2);
                finalRotation2 = newRotation * Quaternion.Euler(0, 0, -hitAngle / 2);
                transform.rotation = finalRotation2;
                lineRenderer.enabled = true;
                boxCollider.enabled = true;
                isHitting = true;
                hitStep = true;
            }

            if (isHitting)
            {
                if (hitStep)
                {
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, finalRotation1, hitSpeed * Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation1.eulerAngles.z + toleranceAngle &&
                        transform.rotation.eulerAngles.z > finalRotation1.eulerAngles.z - toleranceAngle)
                    {
                        hitStep = false;
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
                        AnimationManagerBool(attackDirectionState, IsAttacking, false);
                        playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
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
        
        private void AnimationControllerBool(int directionState, int parameterToChange)
        {
            AnimationManagerBool(directionState, parameterToChange, true);
            StartCoroutine(playerController.CanChangeCoroutine());
        }
        
        private void AnimationManagerBool(int directionState, int parameterToChange, bool value)
        {
            if (parameterToChange  == IsAttacking)
            {
                playerController.AnimationManagerSwitch(directionState, playerController.currentAnimPrefabAnimator.GetInteger(MovingState), value);
            }

            foreach (var prefab in playerController.animPrefabs.Where(prefab => prefab != playerController.currentAnimPrefab))
            {
                prefab.SetActive(false);
            }
        }
    }
}
