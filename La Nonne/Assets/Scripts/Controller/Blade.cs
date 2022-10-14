using UnityEngine;

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
        public Quaternion finalRotation1;
        public Quaternion finalRotation2;
        private Camera camera1;

        // Start is called before the first frame update
        void Start()
        {
            camera1 = Camera.main;
            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
            isHitting = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !isHitting)
            {
                Vector3 newDirection = camera1!.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                newDirection.z = 0;
                newDirection.Normalize();
                Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, newDirection);
                finalRotation1 = newRotation * Quaternion.Euler(0, 0, hitAngle/2);
                finalRotation2 = newRotation * Quaternion.Euler(0, 0, -hitAngle/2);
                transform.rotation = finalRotation2;
                lineRenderer.enabled = true;
                isHitting = true;
                hitStep = true;
            }
            if (isHitting)
            {
                if (hitStep)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation1, hitSpeed*Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation1.eulerAngles.z + toleranceAngle && transform.rotation.eulerAngles.z > finalRotation1.eulerAngles.z - toleranceAngle)
                    {
                        hitStep = false;
                    }
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRotation2, hitSpeed*Time.deltaTime);
                    if (transform.rotation.eulerAngles.z < finalRotation2.eulerAngles.z + toleranceAngle && transform.rotation.eulerAngles.z > finalRotation2.eulerAngles.z - toleranceAngle)
                    {
                        isHitting = false;
                        lineRenderer.enabled = false;
                    }
                }
            }
        }
    }
}
