using System.Linq;
using AI;
using AI.Boss;
using Shop;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class ChainBladeDamage : MonoBehaviour
    {
        public float damageAndEffectMultiplier = 1f;

        [FormerlySerializedAs("SO_Controller")] public SO_Controller soController;
        private EffectManager effectManager;
        
        private void Start()
        {
            effectManager = EffectManager.instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var o = other.gameObject;
            var damage = (int)(soController.playerAttackDamage * damageAndEffectMultiplier);
            
            //DMG du player sur le TrashMobClose
            if (o.CompareTag("Enemy"))
            {
                o.GetComponent<EnemyController>().TakeDamageFromPlayer(damage);
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
                ImplodeStacks(o, damage);
            }
            
            //DMG du player sur le BOSS
            if (o.CompareTag("Boss"))
            {
                o.GetComponent<BossStateManager>().TakeDamageOnBossFromPlayer(damage);
                ImplodeStacks(o, damage);
            }
        }

        private void ImplodeStacks(GameObject enemy, int damage)
        {
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyStacks = enemy.GetComponent<EnemyController>().stacks;
                    
                    if (enemyStacks.All(leveledEffect => leveledEffect.effect != EffectManager.Effect.None))
                    {
                        for (var i = 0; i < enemyStacks.Length; i++)
                        {
                            effectManager.SuperEffectSwitch(enemyStacks[i].effect, enemyStacks[i].level, gameObject, damage, damageAndEffectMultiplier);
                            enemy.GetComponent<EnemyController>().stacks[i].effect = EffectManager.Effect.None;
                        }
                    }
                    
                    break;
                
                case "Boss":
                    var bossStacks = enemy.GetComponent<BossStateManager>().stacks;
                    
                    if (bossStacks.All(leveledEffect => leveledEffect.effect != EffectManager.Effect.None))
                    {
                        for (var i = 0; i < bossStacks.Length; i++)
                        {
                            effectManager.SuperEffectSwitch(bossStacks[i].effect, bossStacks[i].level, gameObject, damage, damageAndEffectMultiplier);
                            enemy.GetComponent<EnemyController>().stacks[i].effect = EffectManager.Effect.None;
                        }
                    }
                    
                    break;
                
                default:
                    return;
            }
        }
    }
}
