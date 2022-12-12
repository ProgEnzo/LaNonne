using AI.So;
using Controller;
using UnityEngine;

namespace AI
{
    public class Bullet : MonoBehaviour
    {
        private PlayerController player;
        [SerializeField] private SoTrashMobRange soTrashMobRange;

        private void Start()
        {
            player = PlayerController.instance;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            //Si les bullet du TrashMobRange touchent le player
            if (col.gameObject.CompareTag("Player"))
            {
                player.TakeDamage(soTrashMobRange.bulletDamage);
                Destroy(gameObject);
                Debug.Log("<color=green>PLAYER</color> HAS BEEN HIT, FOR THIS AMOUNT OF DMG : " + soTrashMobRange.bulletDamage);
            
            }
        }

 
    }
}
