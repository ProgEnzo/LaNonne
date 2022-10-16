using Controller;
using UnityEngine;

namespace AI
{
    public class Bullet : MonoBehaviour
    {

        [SerializeField] private GameObject player;
        [SerializeField] private int bulletDamage;

        private PlayerController playerController;
        private void OnTriggerEnter2D(Collider2D col)
        {
            //Si les bullet du TrashMobRange touchent le player
            if (col.gameObject.CompareTag("Player"))
            {
                player.GetComponent<PlayerController>().TakeDamage(bulletDamage);
                Destroy(gameObject);
                Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, FOR THIS AMOUNT OF DMG : " + bulletDamage);
            
            }
        }

 
    }
}
