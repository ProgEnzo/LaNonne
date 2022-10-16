using AI.Elite;
using AI.Trash;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class Blade : MonoBehaviour
    {
        public bool isHitting;
        public bool hitStep;
        public float hitAngle = 45f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public LineRenderer lineRenderer;
        public BoxCollider2D boxCollider;
        public Quaternion finalRotation1;
        public Quaternion finalRotation2;
        private Camera camera1;

        public ChainBladeDamage chainBladeDamage;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        void Start()
        {
            camera1 = Camera.main;
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            boxCollider = gameObject.GetComponent<BoxCollider2D>();
            lineRenderer.enabled = false;
            boxCollider.enabled = false;
            isHitting = false;
        }

        // Update is called once per frame
        void Update()
        {
            ZealousBlade();
        }

        public void ZealousBlade()
        {
            if (Input.GetMouseButtonDown(0) && !isHitting)
            {
                Vector3 newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
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
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //DMG du player sur le TrashMobClose
            if (other.gameObject.CompareTag("TrashMobClose"))
            {
                other.gameObject.GetComponent<TrashMobClose>().TakeDamageFromPlayer(soController.playerAttackDamage);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }

            //DMG du player sur le TrashMobRange
            if (other.gameObject.CompareTag("TrashMobRange"))
            {
                other.gameObject.GetComponent<TrashMobRange>().TakeDamageFromPlayer(soController.playerAttackDamage);
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //DMG du player sur le Bully
            if (other.gameObject.CompareTag("Bully"))
            {
                other.gameObject.GetComponent<Bully>().TakeDamageFromPlayer(soController.playerAttackDamage);
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //DMG du player sur le caretaker
            if (other.gameObject.CompareTag("Caretaker"))
            {
                other.gameObject.GetComponent<CareTaker>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * chainBladeDamage.damageMultiplier));
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
        }
    }
}
