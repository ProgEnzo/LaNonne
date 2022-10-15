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
            
            //DMG du player sur le TDI
            if (other.gameObject.CompareTag("TDI"))
            {
                other.gameObject.GetComponent<TDI>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                //Debug.Log("<color=red>TRASH MOB RANGE</color>TRASH MOB HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobRange>().currentHealth);
            }
        }
    }
}
