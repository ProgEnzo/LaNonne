using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBlade : MonoBehaviour
    {
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        private bool isHitting;
        private LineRenderer chainLineRenderer;
        private LineRenderer bladeLineRenderer;
        private BoxCollider2D chainBoxCollider;
        private BoxCollider2D bladeBoxCollider;
        private Quaternion initialRotation;
        private Quaternion finalRotation;
        private Camera camera1;
        private PlayerController playerController;
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private int attackDirectionState;
        private float playerScale;
        private float localStateMult;
        private float currentTime;
        
        private AnimationManager animationManager;
        private InputManager inputManager;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            animationManager = AnimationManager.instance;
            inputManager = InputManager.instance;
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

            if (Input.GetKeyDown(inputManager.inquisitorialChainKey) && !isHitting && currentTime <= 0)
            {
                InquisitorialChainStart();
            }

            chainLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(0, new Vector3(0, (soController.inquisitorialChainChainHitLength-soController.inquisitorialChainBladeHitLength)/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            currentTime -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            InquisitorialChain();
            
            chainBoxCollider.size = new Vector2(0.1f/parentLocalScaleX, soController.inquisitorialChainChainHitLength/parentLocalScaleX);
            chainBoxCollider.offset = new Vector2(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX/2);
            bladeBoxCollider.size = new Vector2(0.11f/parentLocalScaleX, soController.inquisitorialChainBladeHitLength/parentLocalScaleX);
            bladeBoxCollider.offset = new Vector2(0, (soController.inquisitorialChainChainHitLength-soController.inquisitorialChainBladeHitLength/2)/parentLocalScaleX);
        }

        private void InquisitorialChainStart()
        {
            currentTime = soController.inquisitorialChainCooldownTime;
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
            var playerTransform = playerController.transform;
            var playerLocalScale = playerTransform.localScale;
            playerTransform.localScale = new Vector3(playerScale * localStateMult,
                playerLocalScale.y, playerLocalScale.z);
            playerController.transform.GetChild(0).localScale = new Vector3(1, 1 * localStateMult, 1);
            animationManager.AnimationControllerBool(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, attackDirectionState, IsAttacking);
            initialRotation = newRotation * Quaternion.Euler(0, 0, soController.inquisitorialChainHitAngle / 2);
            finalRotation = newRotation * Quaternion.Euler(0, 0, -soController.inquisitorialChainHitAngle / 2);
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
                    Quaternion.RotateTowards(transform.rotation, finalRotation, soController.inquisitorialChainHitSpeed * Time.deltaTime);
                if (transform.rotation.eulerAngles.z < finalRotation.eulerAngles.z + soController.inquisitorialChainToleranceAngle &&
                    transform.rotation.eulerAngles.z > finalRotation.eulerAngles.z - soController.inquisitorialChainToleranceAngle)
                {
                    isHitting = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    AnimationManager.AnimationManagerBool(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, attackDirectionState, IsAttacking, false);
                    playerController.currentAnimPrefabAnimator.SetBool(IsAttacking, false);
                }
            }
        }
    }
}
