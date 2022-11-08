using AI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBladeDamage : MonoBehaviour
    {
        public float damageMultiplier = 1f;

        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        [FormerlySerializedAs("SO_Enemy")] public SO_Enemy soEnemy;

        private void OnTriggerEnter2D(Collider2D other)
        {
            //DMG du player sur le TrashMobClose
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyController>().TakeDamageFromPlayer((int)(soController.playerAttackDamage * damageMultiplier));
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
            }
            
            //DMG du player sur le BOSS
            if (other.gameObject.CompareTag("Boss"))
            {
                other.gameObject.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(soController.playerAttackDamage);
            }
        }
    }
}
