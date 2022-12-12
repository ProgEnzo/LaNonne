using UnityEngine;

namespace Controller
{
    public class EPCount : MonoBehaviour
    {
        [SerializeField] private int epValue;
        [SerializeField] private PlayerController playerController;

        void Start()
        {
            playerController = PlayerController.instance;
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                playerController.AddEp(epValue);

                Destroy(gameObject);
            }
        }
    
    
    }
}
