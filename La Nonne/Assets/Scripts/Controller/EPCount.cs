using Manager;
using UnityEngine;

namespace Controller
{
    public class EPCount : MonoBehaviour
    {
        [SerializeField] private int epValue;
        [SerializeField] private PlayerController playerController;
        private ScoreManager scoreManager;
        private void Start()
        {
            playerController = PlayerController.instance;
            scoreManager = ScoreManager.instance;
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                playerController.AddEp(epValue);
                scoreManager.AddEpScore(1);
                Destroy(gameObject);
            }
        }
    }
}
