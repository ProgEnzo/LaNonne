using System;
using System.Collections;
using DG.Tweening;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controller
{
    public class ChainBlade : MonoBehaviour
    {
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        private bool isHitting;
        internal bool isWarningOn;
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
        private GameObject chainGameObject;
        private GameObject bladeGameObject;
        private GameObject warningGameObject;
        private Animator animator;

        private AnimationManager animationManager;
        private InputManager inputManager;
        private UIManager uiManager;
        private CamManager camManager;
        
        [Header("SoundEffect")] 
        public AudioSource whipchainAudioSource;
        public AudioClip whipchainSound;
        public AudioClip whipchainCooldownSound;
        private bool hasDoneFullCooldownBehavior;
        private static readonly int WhipAnimState = Animator.StringToHash("whipAnimState");


        // Start is called before the first frame update
        private void Start()
        {
            playerController = PlayerController.instance;
            animationManager = AnimationManager.instance;
            inputManager = InputManager.instance;
            uiManager = UIManager.instance;
            
            camera1 = Camera.main;
            chainGameObject = transform.GetChild(0).gameObject;
            bladeGameObject = transform.GetChild(1).gameObject;
            warningGameObject = transform.GetChild(2).gameObject;
            chainLineRenderer = chainGameObject.GetComponent<LineRenderer>();
            bladeLineRenderer = bladeGameObject.GetComponent<LineRenderer>();
            chainBoxCollider = chainGameObject.GetComponent<BoxCollider2D>();
            bladeBoxCollider = bladeGameObject.GetComponent<BoxCollider2D>();
            chainGameObject.SetActive(false);
            bladeGameObject.SetActive(false);
            isHitting = false;
            isWarningOn = false;
            playerScale = playerController.transform.localScale.x;
            currentTime = 0;
            inquisitorialChainCooldownBar = GameObject.Find("InquisitorialChainCooldown").GetComponent<Image>();
            inquisitorialChainCooldownBar.fillAmount = 1f;
            hasDoneFullCooldownBehavior = true;
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            var parentLocalScale = transform.parent.localScale;
            var warningScale = warningGameObject.transform.localScale;

            if (Input.GetKeyDown(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && currentTime <= 0 && !uiManager.IsAnyMenuOpened())
            {
                playerController.currentSlowMoPlayerMoveSpeedFactor = 0.15f;
                Time.timeScale = 0.15f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                warningGameObject.SetActive(true);
                warningGameObject.transform.localScale = new Vector3(warningScale.x * parentLocalScale.x, warningScale.y * parentLocalScale.y, warningScale.z * parentLocalScale.z);
                isWarningOn = true;
                //CamManager.instance.ChainBladeCamState(1);
            }
            if (Input.GetKey(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && isWarningOn && !uiManager.IsAnyMenuOpened())
            {
                var newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                warningGameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                warningGameObject.transform.localScale = new Vector3(Mathf.Abs(warningScale.x) * parentLocalScale.x, Mathf.Abs(warningScale.y) * parentLocalScale.y, Mathf.Abs(warningScale.z) * parentLocalScale.z);
            }
            if (Input.GetKeyUp(inputManager.inquisitorialChainKey) && !playerController.isRevealingDashOn && isWarningOn && !uiManager.IsAnyMenuOpened())
            {
                whipchainAudioSource.PlayOneShot(whipchainSound);
                
                isWarningOn = false;
                warningGameObject.transform.localScale = new Vector3(Mathf.Abs(warningScale.x), Mathf.Abs(warningScale.y), Mathf.Abs(warningScale.z));
                warningGameObject.SetActive(false);
                playerController.currentSlowMoPlayerMoveSpeedFactor = 1f;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                InquisitorialChainStart();
                //CamManager.instance.ChainBladeCamState(2);
            }
            
            if (uiManager.isGamePaused && isWarningOn)
            {
                warningGameObject.SetActive(false);
                chainGameObject.SetActive(false);
                bladeGameObject.SetActive(false);
                isHitting = false;
                isWarningOn = false;
                playerController.currentSlowMoPlayerMoveSpeedFactor = 1f;
            }
            
            if (Input.GetKeyDown(inputManager.dashKey) && isWarningOn)
            {
                warningGameObject.SetActive(false);
                chainGameObject.SetActive(false);
                bladeGameObject.SetActive(false);
                isHitting = false;
                isWarningOn = false;
                playerController.currentSlowMoPlayerMoveSpeedFactor = 1f;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }

            chainLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(0, new Vector3(0, (soController.inquisitorialChainChainHitLength-soController.inquisitorialChainBladeHitLength)/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(1, new Vector3(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX, 0));
            currentTime -= Time.deltaTime;
            inquisitorialChainCooldownBar.fillAmount = 1 - currentTime / soController.inquisitorialChainCooldownTime;

            if (currentTime <= 0 && !hasDoneFullCooldownBehavior)
            {
                whipchainAudioSource.PlayOneShot(whipchainCooldownSound);
                //Do some shit
                var duplicateRevealingImage = Instantiate(inquisitorialChainCooldownBar.transform.parent.gameObject, inquisitorialChainCooldownBar.transform.position, Quaternion.identity, inquisitorialChainCooldownBar.transform.parent.parent);
                duplicateRevealingImage.GetComponent<RectTransform>().DOScale(new Vector3(3f, 3f, 3f), 1f);
                duplicateRevealingImage.GetComponent<Image>().DOFade(0f, 1f);
                duplicateRevealingImage.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 1f);
                Destroy(duplicateRevealingImage, 1f);
                hasDoneFullCooldownBehavior = true;
            }
        }

        private void FixedUpdate()
        {
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            
            InquisitorialChain();
            
            chainBoxCollider.size = new Vector2(0.1f/MathF.Abs(parentLocalScaleX), soController.inquisitorialChainChainHitLength/MathF.Abs(parentLocalScaleX));
            chainBoxCollider.offset = new Vector2(0, soController.inquisitorialChainChainHitLength/parentLocalScaleX/2);
            bladeBoxCollider.size = new Vector2(0.11f/MathF.Abs(parentLocalScaleX), soController.inquisitorialChainBladeHitLength/MathF.Abs(parentLocalScaleX));
            bladeBoxCollider.offset = new Vector2(0, (soController.inquisitorialChainChainHitLength-soController.inquisitorialChainBladeHitLength/2)/parentLocalScaleX);
        }

        private void InquisitorialChainStart()
        {
            //StartCoroutine(ChangeAnimationState(1));
            currentTime = soController.inquisitorialChainCooldownTime;
            hasDoneFullCooldownBehavior = false;
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
            chainGameObject.SetActive(true);
            bladeGameObject.SetActive(true);
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
                    chainGameObject.SetActive(false);
                    bladeGameObject.SetActive(false);
                    animationManager.AnimationControllerPlayer(playerController.animPrefabs, ref playerController.currentAnimPrefab, ref playerController.currentAnimPrefabAnimator, AttackState, 0);
                    CamManager.instance.ChainBladeCamShakeState();
                }
            }
        }
        
        private IEnumerator ChangeAnimationState(int state)
        {
            animator.SetInteger(WhipAnimState, state);
            yield return new WaitForNextFrameUnit();
            animator.SetInteger(WhipAnimState, 0);
        }
    }
}
