using System.Collections;
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
        public int epCost;
        private Animator playerAnimator;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        private void Start()
        {
            playerAnimator = PlayerController.instance.GetComponent<Animator>();
            camera1 = Camera.main;
            chainLineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
            bladeLineRenderer = transform.GetChild(1).GetComponent<LineRenderer>();
            chainBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
            bladeBoxCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            isHitting = false;
        }

        // Update is called once per frame
        private void Update()
        {
            InquisitorialChain();
            var parentLocalScaleX = transform.parent.parent.localScale.x;
            chainLineRenderer.SetPosition(1, new Vector3(0, chainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(0, new Vector3(0, chainHitLength/parentLocalScaleX, 0));
            bladeLineRenderer.SetPosition(1, new Vector3(0, (chainHitLength+bladeHitLength)/parentLocalScaleX, 0));
            chainBoxCollider.size = new Vector2(0.1f/parentLocalScaleX, chainHitLength/parentLocalScaleX);
            chainBoxCollider.offset = new Vector2(0, chainHitLength/parentLocalScaleX/2);
            bladeBoxCollider.size = new Vector2(0.1f/parentLocalScaleX, bladeHitLength/parentLocalScaleX);
            bladeBoxCollider.offset = new Vector2(0, (chainHitLength+bladeHitLength/2)/parentLocalScaleX);
        }

        private void InquisitorialChain()
        {
            if (Input.GetMouseButtonDown(1) && !isHitting && soController.epAmount >= epCost)
            {
                StartCoroutine(AnimationControllerBool(IsAttacking));
                soController.epAmount -= epCost;
                Vector3 newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                initialRotation = newRotation * Quaternion.Euler(0, 0, hitAngle / 2);
                finalRotation = newRotation * Quaternion.Euler(0, 0, -hitAngle / 2);
                transform.rotation = initialRotation;
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
                isHitting = true;
            }

            if (isHitting)
            {
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, finalRotation, hitSpeed * Time.deltaTime);
                if (transform.rotation.eulerAngles.z < finalRotation.eulerAngles.z + toleranceAngle &&
                    transform.rotation.eulerAngles.z > finalRotation.eulerAngles.z - toleranceAngle)
                {
                    isHitting = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                    playerAnimator.SetBool(IsAttacking, false);
                }
            }
        }
        
        private IEnumerator AnimationControllerBool(int parameterToChange)
        {
            playerAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            playerAnimator.SetBool(CanChange, false);
            playerAnimator.SetBool(parameterToChange, true);
        }
    }
}
