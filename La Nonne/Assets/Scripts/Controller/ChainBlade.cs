using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBlade : MonoBehaviour
    {
        public bool isHitting;
        public float hitAngle = 100f;
        public float hitSpeed = 1f;
        public float toleranceAngle = 1f;
        public Quaternion initialRotation;
        public Quaternion finalRotation;
        private Camera camera1;
        public int epCost;
    
        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        // Start is called before the first frame update
        void Start()
        {
            camera1 = Camera.main;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            isHitting = false;
        }

        // Update is called once per frame
        void Update()
        {
            InquisitorialChain();
        }

        public void InquisitorialChain()
        {
            if (Input.GetMouseButtonDown(1) && !isHitting && soController.epAmount >= epCost)
            {
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
                }
            }
        }
    }
}
