using AI;
using AI.Boss;
using Manager;
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
        private ScoreManager scoreManager;
        
        private void Start()
        {
            effectManager = EffectManager.instance;
            scoreManager = ScoreManager.instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var o = other.gameObject;
            var damage = (int)(soController.inquisitorialChainDamage * damageAndEffectMultiplier);
            
            //DMG du player on Enemy
            if (o.CompareTag("Enemy"))
            {
                var enemyScript = o.GetComponent<EnemyController>();
                enemyScript.TakeDamageFromPlayer(damage);
                enemyScript.HitStop(soController.inquisitorialChainHitStopDuration);
                scoreManager.AddChainBladeHitScore(5);
                
                //Debug.Log("<color=orange>TRASH MOB CLOSE</color> HAS BEEN HIT, HEALTH REMAINING : " + other.gameObject.GetComponent<TrashMobClose>().currentHealth);
                ImplodeStacks(o, damage);
            }
            
            //DMG du player sur le BOSS
            if (o.CompareTag("Boss"))
            {
                var bossScript = o.GetComponent<BossStateManager>();
                bossScript.TakeDamageOnBossFromPlayer(damage);
                bossScript.HitStop(soController.inquisitorialChainHitStopDuration);
                scoreManager.AddChainBladeHitScore(5);
                
                ImplodeStacks(o, damage);
            }
        }

        private void ImplodeStacks(GameObject enemy, int damage)
        {
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyStacks = enemy.GetComponent<EnemyController>().stacks;
                    
                    for (var i = 0; i < enemyStacks.Length; i++)
                    {
                        if (enemyStacks[i].effect == EffectManager.Effect.None) continue;
                        effectManager.SuperEffectSwitch(enemyStacks[i].effect, enemyStacks[i].level, enemy, damage, damageAndEffectMultiplier);
                        enemy.GetComponent<EnemyController>().stacks[i].effect = EffectManager.Effect.None;
                    }
                    
                    break;
                
                case "Boss":
                    var bossStacks = enemy.GetComponent<BossStateManager>().stacks;
                    
                    for (var i = 0; i < bossStacks.Length; i++)
                    {
                        if (bossStacks[i].effect == EffectManager.Effect.None) continue;
                        effectManager.SuperEffectSwitch(bossStacks[i].effect, bossStacks[i].level, gameObject, damage, damageAndEffectMultiplier);
                        enemy.GetComponent<EnemyController>().stacks[i].effect = EffectManager.Effect.None;
                    }
                    
                    break;
                
                default:
                    return;
            }
        }
    }
}
