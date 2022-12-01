using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Boss;
using Core.Scripts.Utils;
using Shop;
using UnityEngine;

[Serializable]
internal class ListOfShopSo
{
    [SerializeField] private List<ShopObjectSO> shopObjects;

    public ShopObjectSO this[int i] => shopObjects[i];
}

namespace Shop
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        internal enum Effect
        {
            None = -1,
            Bleed = 0,
            Chill = 1,
            Target = 2,
            Wealth = 3
        }

        internal new static EffectManager instance;
        
        [SerializeField] internal float effectDuration;
        [SerializeField] internal int effectMaxLevel;
        [SerializeField] internal int numberOfEffects;
        [SerializeField] internal List<ListOfShopSo> effectDictionary = new();
        internal readonly Dictionary<Effect, int> effectInventory = new();
        internal readonly Effect[] appliedEffects = new Effect[3];


        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instance = this;
            }
            
            for (var i = 0; i < numberOfEffects; i++)
            {
                effectInventory.Add((Effect)i, 0);
            }
            for (var i = 0; i < appliedEffects.Length; i++)
            {
                appliedEffects[i] = Effect.None;
            }
        }
        
        internal void EffectSwitch(Effect effect, int level, GameObject enemy, int stackIndex)
        {
            switch (effect)
            {
                case Effect.Bleed:
                    StartCoroutine(Bleed(level, enemy, stackIndex));
                    break;
                case Effect.Chill:
                    StartCoroutine(Chill(level, enemy, stackIndex));
                    break;
                case Effect.Target:
                    StartCoroutine(Target(level, enemy, stackIndex));
                    break;
                case Effect.Wealth:
                    StartCoroutine(Wealth(level, enemy, stackIndex));
                    break;
                default:
                    Debug.Log("None");
                    break;
            }
        }
        
        private static IEnumerator Bleed(int level, GameObject enemy, int stackIndex)
        {
            var bleedSo = (BleedSO) instance.effectDictionary[(int)Effect.Bleed][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    yield return new WaitForSeconds(bleedSo.cooldown);
                    while (enemyController.stacks[stackIndex].effect == Effect.Bleed)
                    {
                        enemyController.TakeDamageFromPlayer(bleedSo.damage);
                        yield return new WaitForSeconds(bleedSo.cooldown);
                    }
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    yield return new WaitForSeconds(bleedSo.cooldown);
                    while (bossController.stacks[stackIndex].effect == Effect.Bleed)
                    {
                        bossController.TakeDamageOnBossFromPlayer(bleedSo.damage);
                        yield return new WaitForSeconds(bleedSo.cooldown);
                    }
                    bossController.areStacksOn[stackIndex] = false;
                    break;
            }
        }
        
        private static IEnumerator Chill(int level, GameObject enemy, int stackIndex)
        {
            var chillSo = (ChillSO) instance.effectDictionary[(int)Effect.Chill][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.currentAiPathSpeed *= chillSo.rateOfBasicSpeed;
                    enemyController.currentVelocitySpeed *= chillSo.rateOfBasicSpeed;
                    bool ConditionEnemy() => enemyController.stacks[stackIndex].effect != Effect.Chill;
                    yield return new WaitUntil(ConditionEnemy);
                    enemyController.currentAiPathSpeed /= chillSo.rateOfBasicSpeed;
                    enemyController.currentVelocitySpeed /= chillSo.rateOfBasicSpeed;
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentAiPathSpeed *= chillSo.rateOfBasicSpeed;
                    bossController.currentVelocitySpeed *= chillSo.rateOfBasicSpeed;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Chill;
                    yield return new WaitUntil(ConditionBoss);
                    bossController.currentAiPathSpeed /= chillSo.rateOfBasicSpeed;
                    bossController.currentVelocitySpeed /= chillSo.rateOfBasicSpeed;
                    bossController.areStacksOn[stackIndex] = false;
                    break;
            }
        }
        
        private static IEnumerator Target(int level, GameObject enemy, int stackIndex)
        {
            var targetSo = (TargetSO) instance.effectDictionary[(int)Effect.Target][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.currentDamageMultiplier *= targetSo.rateOfDamage;
                    bool ConditionEnemy() => enemyController.stacks[stackIndex].effect != Effect.Target;
                    yield return new WaitUntil(ConditionEnemy);
                    enemyController.currentDamageMultiplier /= targetSo.rateOfDamage;
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentDamageMultiplier *= targetSo.rateOfDamage;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Target;
                    yield return new WaitUntil(ConditionBoss);
                    bossController.currentDamageMultiplier /= targetSo.rateOfDamage;
                    bossController.areStacksOn[stackIndex] = false;
                    break;
            }
        }
        
        private static IEnumerator Wealth(int level, GameObject enemy, int stackIndex)
        {
            var wealthSo = (WealthSO) instance.effectDictionary[(int)Effect.Wealth][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.currentEpDropMultiplier *= wealthSo.epDropRate;
                    bool ConditionEnemy() => enemyController.stacks[stackIndex].effect != Effect.Wealth;
                    yield return new WaitUntil(ConditionEnemy);
                    enemyController.currentEpDropMultiplier /= wealthSo.epDropRate;
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentEpDropMultiplier *= wealthSo.epDropRate;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Wealth;
                    yield return new WaitUntil(ConditionBoss);
                    bossController.currentEpDropMultiplier /= wealthSo.epDropRate;
                    bossController.areStacksOn[stackIndex] = false;
                    break;
            }
        }
    }
}
