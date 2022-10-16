using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBladeDamage : MonoBehaviour
    {
        public float damageMultiplier = 1f;

        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;

        private void OnTriggerEnter2D(Collider2D other)
        {
            //DMG du player sur le TrashMobClose
            if (other.gameObject.CompareTag("TrashMobClose"))
            {
                other.gameObject.GetComponent<TrashMobClose>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }

            //DMG du player sur le TrashMobRange
            if (other.gameObject.CompareTag("TrashMobRange"))
            {
                other.gameObject.GetComponent<TrashMobRange>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //DMG du player sur le Bully
            if (other.gameObject.CompareTag("Bully"))
            {
                other.gameObject.GetComponent<Bully>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
            
            //DMG du player sur le caretaker
            if (other.gameObject.CompareTag("Caretaker"))
            {
                other.gameObject.GetComponent<CareTaker>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
        }
    }
}
