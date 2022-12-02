using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBlade : MonoBehaviour
    {
        public bool isHitting;
        public float chainHitLength;
        public float bladeHitLength;
        public float hitAngle = 100f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public LineRenderer chainLineRenderer;
        public LineRenderer bladeLineRenderer;
        public BoxCollider2D chainBoxCollider;
        public BoxCollider2D bladeBoxCollider;
        public Quaternion initialRotation;
        public Quaternion finalRotation;
        private Camera camera1;
        private PlayerController playerController;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int MovingState = Animator.StringToHash("movingState");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private int attackDirectionState;
        private float playerScale;
        private float localStateMult;
        [SerializeField] private float cooldownTime;
        private float currentTime;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            camera1 = Camera.main;
            chainLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
            bladeLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
            chainBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
            bladeBoxCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            isHitting = false;
            playerScale = playerController.transform.localScale.x;
            currentTime = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;

            if (Input.GetMouseButtonDown(1) && !isHitting && currentTime <= 0)
            {
                InquisitorialChainStart();
            }

            chainLineRenderer.SetPosition(1, new Vector3(0, chainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(0, new Vector3(0, (chainHitLength-bladeHitLength)/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(1, new Vector3(0, chainHitLength/parentLocalScaleX, 0));
            currentTime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            InquisitorialChain();
            
            chainBoxCollider.size = new Vector2(0.1f/parentLocalScaleX, chainHitLength/parentLocalScaleX);
            chainBoxCollider.offset = new Vector2(0, chainHitLength/parentLocalScaleX/2);
            bladeBoxCollider.size = new Vector2(0.1f/parentLocalScaleX, bladeHitLength/parentLocalScaleX);
            bladeBoxCollider.offset = new Vector2(0, (chainHitLength-bladeHitLength/2)/parentLocalScaleX);
        }

        private void InquisitorialChainStart()
        {
            currentTime = cooldownTime;
            var newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
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
            playerController.transform.localScale = new Vector3(playerScale * localStateMult,
                playerController.transform.localScale.y, playerController.transform.localScale.z);
            playerController.transform.GetChild(0).localScale = new Vector3(1, 1 * localStateMult, 1);
            AnimationControllerBool(attackDirectionState, IsAttacking);
            initialRotation = newRotation * Quaternion.Euler(0, 0, hitAngle / 2);
            finalRotation = newRotation * Quaternion.Euler(0, 0, -hitAngle / 2);
            transform.rotation = initialRotation;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            isHitting = true;
        }

        private void InquisitorialChain()
        {
            if (isHitting)
            {
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, finalRotation, hitSpeed * Time.deltaTime);
                if (transform.rotation.eulerAngles.z < finalRotation.eulerAngles.z + toleranceAngle &&
                    transform.rotation.eulerAngles.z > finalRotation.eulerAngles.z - toleranceAngle)
                {
                    isHitting = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);;
                    AnimationManagerBool(attackDirectionState, IsAttacking, false);
                    playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                }
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
