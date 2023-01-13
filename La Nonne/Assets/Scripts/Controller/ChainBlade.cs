using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controller
{
    public class ChainBlade : MonoBehaviour
    {
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        private bool isHitting;
        private bool isWarningOn;
        private LineRenderer chainLineRenderer;
        private LineRenderer bladeLineRenderer;
        private BoxCollider2D chainBoxCollider;
        private BoxCollider2D bladeBoxCollider;
        private Quaternion initialRotation;
        private Quaternion finalRotation;
        private Camera camera1;
        private PlayerController playerController;
        private static readonly int DirectionState = Animator.StringToHash("directionState");
        private static readonly int AttackState = Animator.StringToHash("attackState");
        private int attackDirectionState;
        private float playerScale;
        private float localStateMult;
        private float currentTime;
        private Image inquisitorialChainCooldownBar;

        private AnimationManager animationManager;
        private InputManager inputManager;
        private UIManager uiManager;
        private CamManager camManager;
        private AudioManager audioManager;

        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            animationManager = AnimationManager.instance;
            inputManager = InputManager.instance;
            uiManager = UIManager.instance;
            audioManager = AudioManager.instance;
            
            camera1 = Camera.main;
            chainLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
            bladeLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
            chainBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
            bladeBoxCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            isHitting = false;
            isWarningOn = false;
            playerScale = playerController.transform.localScale.x;
            currentTime = 0;
            inquisitorialChainCooldownBar = GameObject.Find("InquisitorialChainCooldown").GetComponent<Image>();
            inquisitorialChainCooldownBar.fillAmount = 1f;
        }

        // Update is called once per frame
        private void Update()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            var parentLocalScale = transform.parent.localScale;
            var warningScale = transform.GetChild(2).localScale;

            if (Input.GetKeyDown(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && currentTime <= 0 && !uiManager.isGamePaused && !uiManager.isShopOpened && !uiManager.isWhipMenuOpened)
            {
                playerController.currentSlowMoPlayerMoveSpeedFactor = 0f;
                Time.timeScale = 0.001f;
                Time.fixedDeltaTime = 0.001f * Time.timeScale;
                transform.GetChild(2).gameObject.SetActive(true);
                transform.GetChild(2).localScale = new Vector3(warningScale.x * parentLocalScale.x, warningScale.y * parentLocalScale.y, warningScale.z * parentLocalScale.z);
                isWarningOn = true;
                //CamManager.instance.ChainBladeCamState(1);
            }
            if (Input.GetKey(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && isWarningOn && !uiManager.isGamePaused && !uiManager.isShopOpened && !uiManager.isWhipMenuOpened)
            {
                var newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                transform.GetChild(2).rotation = Quaternion.LookRotation(Vector3.forward, newDirection);
            }
            if (Input.GetKeyUp(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && isWarningOn && !uiManager.isGamePaused && !uiManager.isShopOpened && !uiManager.isWhipMenuOpened)
            {
                audioManager.PlaySound("WhipchainNoHit");
                
                isWarningOn = false;
                transform.GetChild(2).localScale = new Vector3(Mathf.Abs(warningScale.x), Mathf.Abs(warningScale.y), Mathf.Abs(warningScale.z));
                transform.GetChild(2).gameObject.SetActive(false);
                playerController.currentSlowMoPlayerMoveSpeedFactor = 1f;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                InquisitorialChainStart();
                //CamManager.instance.ChainBladeCamState(2);
            }
            
            if (uiManager.isGamePaused && isWarningOn)
            {
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                isHitting = false;
                isWarningOn = false;
                playerController.currentSlowMoPlayerMoveSpeedFactor = 1f;
            }

            chainLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(0, new Vector3(0, (soController.inquisitorialChainChainHitLength-soController.inquisitorialChainBladeHitLength)/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            currentTime -= Time.deltaTime;
            inquisitorialChainCooldownBar.fillAmount = 1 - currentTime / soController.inquisitorialChainCooldownTime;
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
            animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, DirectionState, attackDirectionState);
            animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, 4);
            CamManager.instance.ChainBladeCamDezoomState(1);
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
                    animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, 0);
                    CamManager.instance.ChainBladeCamShakeState();
                }
            }
        }
    }
}
