using System.Collections;
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
        private Animator playerAnimator;
        private static readonly int CanChange = Animator.StringToHash("canChange");
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

        public ChainBladeDamage chainBladeDamage;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        private void Start()
        {
            playerAnimator = PlayerController.instance.GetComponent<Animator>();
            camera1 = Camera.main;
            lineRenderer = GetComponent<LineRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
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
                StartCoroutine(AnimationControllerBool(IsAttacking));
                Vector3 newDirection = camera1.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
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
                        playerAnimator.SetBool(IsAttacking, false);
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
        
        private IEnumerator AnimationControllerBool(int parameterToChange)
        {
            playerAnimator.SetBool(CanChange, true);
            yield return new WaitForNextFrameUnit();
            playerAnimator.SetBool(CanChange, false);
            playerAnimator.SetBool(parameterToChange, true);
        }
    }
}
