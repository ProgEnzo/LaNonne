using Controller;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.Elite
{
    public class HealCircle : MonoBehaviour
    {
        [SerializeField] private EnemyController healer;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!healer.isStunned)
            {
                //Heal le TrashMobCLose
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<EnemyController>().currentHealth += healer is CareTaker taker ? taker.soCaretaker.healAmount : ((TDI)healer).soTdi.healAmount;
                    //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + col.gameObject.GetComponent<TrashMobClose>().currentHealth);
                    
                    GameObject.Find("ParticleCaretakerHealZone").GetComponent<ParticleSystem>().Play();
                }

                if (col.gameObject.CompareTag("Player"))
                {
                    col.GetComponent<PlayerController>().TakeDamage(healer is CareTaker taker ? taker.soCaretaker.circleDamage : ((TDI)healer).soTdi.circleDamage); //Player takes damage
                }
            }
        }
    }
}
