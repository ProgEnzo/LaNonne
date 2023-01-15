using System.Collections;
using Controller;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Help
{
    public class PlayerHelp : MonoBehaviour
    {
        public GameObject epHelp;

        private RoomContentGenerator roomContentGenerator;
        private Rigidbody2D rb;
        private UIManager uiManager;
        private PlayerController playerController;

        public float speedForBouboule = 3f;
        public float timeToDestroyBouboule = 2f;
    
        public float coolDownBeforeNextBouboule = 4f;
        public float currentTimeBeforeNextBouboule;

        public void OnEnable()
        {
            uiManager = UIManager.instance;
            playerController = PlayerController.instance;
            
            if (GameObject.Find("RoomContentGenerator") != null)
            {
                roomContentGenerator = GameObject.Find("RoomContentGenerator").GetComponent<RoomContentGenerator>();
            }
            currentTimeBeforeNextBouboule = 0f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && currentTimeBeforeNextBouboule <= 0f && !uiManager.IsAnyMenuOpened() && !playerController.isRevealingDashOn && !playerController.chainBlade.isWarningOn)
            {
                rb = Instantiate(epHelp, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
                if (roomContentGenerator != null)
                {
                    rb.velocity = (roomContentGenerator.mapBoss - (Vector2)transform.position).normalized * speedForBouboule;
                }
                else
                {
                    rb.velocity = Random.insideUnitCircle.normalized * speedForBouboule;
                }
                StartCoroutine(DetruitLaBouboule());
                currentTimeBeforeNextBouboule = coolDownBeforeNextBouboule;
            }
            currentTimeBeforeNextBouboule -= Time.deltaTime;
        }

        private IEnumerator DetruitLaBouboule()
        {
            yield return new WaitForSeconds(timeToDestroyBouboule);
            Destroy(rb.gameObject);
        }
    }
}