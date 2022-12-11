using TMPro;
using UnityEngine;

namespace Controller
{
    public class EPCount : MonoBehaviour
    {
        [SerializeField] private int epValue;
        [SerializeField] private TextMeshProUGUI epCount;
        [SerializeField] private PlayerController playerController;

        void Start()
        {
            playerController = PlayerController.instance;
            epCount = GameObject.Find("EP").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("WallCollider"))
            {
                playerController.AddEp(epValue);
                epCount.text = "EP COUNT : " + playerController.currentEp;

                Destroy(gameObject);
            }
        }
    
    
    }
}
