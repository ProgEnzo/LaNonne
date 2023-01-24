using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.Boss;
using Controller;
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
        private readonly Dictionary<GameObject, int> currentFreezeCoroutineOnEnemies = new();


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
        
        internal void SuperEffectSwitch(Effect effect, int level, GameObject enemy, int damage, float multiplier)
        {
            switch (effect)
            {
                case Effect.Bleed:
                    Vampirism(level, damage, multiplier);
                    break;
                case Effect.Chill:
                    if (!currentFreezeCoroutineOnEnemies.ContainsKey(enemy))
                    {
                        currentFreezeCoroutineOnEnemies.Add(enemy,  0);
                        StartCoroutine(Freeze(level, enemy, multiplier));
                    }
                    else
                    {
                        currentFreezeCoroutineOnEnemies[enemy]++;
                    }
                    break;
                case Effect.Target:
                    Burst(level, enemy, multiplier);
                    break;
                case Effect.Wealth:
                    Profusion(level, enemy, multiplier);
                    break;
                default:
                    Debug.Log("None");
                    break;
            }
        }

        #region Effects

        private static IEnumerator Bleed(int level, GameObject enemy, int stackIndex)
        {
            var bleedSo = (BleedSO) instance.effectDictionary[(int)Effect.Bleed][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    yield return new WaitForSeconds(bleedSo.cooldown);
                    if (enemyController == null) yield break;
                    while (enemyController.stacks[stackIndex].effect == Effect.Bleed)
                    {
                        enemyController.TakeDamageFromPlayer(bleedSo.damage);
                        EffectVFXManager(enemyController, Effect.Bleed, false);
                        yield return new WaitForSeconds(bleedSo.cooldown);
                        if (enemyController == null) yield break;
                    }
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    yield return new WaitForSeconds(bleedSo.cooldown);
                    if (bossController == null) yield break;
                    while (bossController.stacks[stackIndex].effect == Effect.Bleed)
                    {
                        bossController.TakeDamageOnBossFromPlayer(bleedSo.damage);
                        yield return new WaitForSeconds(bleedSo.cooldown);
                        if (bossController == null) yield break;
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
                    EffectVFXManager(enemyController, Effect.Chill, false);
                    bool ConditionEnemy() => enemyController.stacks[stackIndex].effect != Effect.Chill;
                    yield return new WaitUntil(ConditionEnemy);
                    if (enemyController == null) yield break;
                    enemyController.currentAiPathSpeed /= chillSo.rateOfBasicSpeed;
                    enemyController.currentVelocitySpeed /= chillSo.rateOfBasicSpeed;
                    EffectVFXManager(enemyController, Effect.Chill, false, false);
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentAiPathSpeed *= chillSo.rateOfBasicSpeed;
                    bossController.currentVelocitySpeed *= chillSo.rateOfBasicSpeed;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Chill;
                    yield return new WaitUntil(ConditionBoss);
                    if (bossController == null) yield break;
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
                    if (enemyController == null) yield break;
                    enemyController.currentDamageMultiplier /= targetSo.rateOfDamage;
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentDamageMultiplier *= targetSo.rateOfDamage;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Target;
                    yield return new WaitUntil(ConditionBoss);
                    if (bossController == null) yield break;
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
                    EffectVFXManager(enemyController, Effect.Wealth, false);
                    bool ConditionEnemy() => enemyController.stacks[stackIndex].effect != Effect.Wealth;
                    yield return new WaitUntil(ConditionEnemy);
                    if (enemyController == null) yield break;
                    enemyController.currentEpDropMultiplier /= wealthSo.epDropRate;
                    EffectVFXManager(enemyController, Effect.Wealth, false, false);
                    enemyController.areStacksOn[stackIndex] = false;
                    break;
                /*case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentEpDropMultiplier *= wealthSo.epDropRate;
                    bool ConditionBoss() => bossController.stacks[stackIndex].effect != Effect.Wealth;
                    yield return new WaitUntil(ConditionBoss);
                    bossController.currentEpDropMultiplier /= wealthSo.epDropRate;
                    bossController.areStacksOn[stackIndex] = false;
                    break;*/
            }
        }

        #endregion
        
        #region SuperEffects
        
        private void Vampirism(int level, int damage, float multiplier)
        {
            var bleedSo = (BleedSO)instance.effectDictionary[(int)Effect.Bleed][level-1];
            PlayerController.instance.HealPlayer((int)(damage * bleedSo.healPart * multiplier));
        }
        
        private IEnumerator Freeze(int level, GameObject enemy, float multiplier)
        {
            var chillSo = (ChillSO)instance.effectDictionary[(int)Effect.Chill][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.currentAiPathSpeed = 0;
                    enemyController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    EffectVFXManager(enemyController, Effect.Chill, true);
                    yield return new WaitForSeconds(chillSo.freezeTime * multiplier);
                    if (currentFreezeCoroutineOnEnemies[enemy] == 0)
                    {
                        enemyController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                        enemyController.currentAiPathSpeed = enemyController.soEnemy.aiPathBasicSpeed;
                        currentFreezeCoroutineOnEnemies.Remove(enemy);
                    }
                    else
                    {
                        currentFreezeCoroutineOnEnemies[enemy]--;
                        StartCoroutine(Freeze(level, enemy, multiplier));
                    }
                    break;
                
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.currentAiPathSpeed = 0;
                    bossController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    yield return new WaitForSeconds(chillSo.freezeTime * multiplier);
                    if (currentFreezeCoroutineOnEnemies[enemy] == 0)
                    {
                        bossController.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                        bossController.currentAiPathSpeed = bossController.bossNormalSpeed;
                    }
                    else
                    {
                        currentFreezeCoroutineOnEnemies[enemy]--;
                        StartCoroutine(Freeze(level, enemy, multiplier));
                    }
                    break;
            }
        }
        
        private static void Burst(int level, GameObject enemy, float multiplier)
        {
            var targetSo = (TargetSO)instance.effectDictionary[(int)Effect.Target][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.TakeDamageFromPlayer((int)(targetSo.burstDamage * multiplier));
                    break;
                case "Boss":
                    var bossController = enemy.GetComponent<BossStateManager>();
                    bossController.TakeDamageOnBossFromPlayer((int)(targetSo.burstDamage * multiplier));
                    break;
            }
        }
        
        private static void Profusion(int level, GameObject enemy, float multiplier)
        {
            var wealthSo = (WealthSO)instance.effectDictionary[(int)Effect.Wealth][level-1];
            switch (enemy.tag)
            {
                case "Enemy":
                    var enemyController = enemy.GetComponent<EnemyController>();
                    enemyController.EpDrop((int)(wealthSo.epExtraDrop * multiplier));
                    break;
            }
        }
        
        #endregion

        #region VFX

        private static void EffectVFXManager(EnemyController enemy, Effect effect, bool isSuperEffect, bool value = true)
        {
            switch (effect)
            {
                case Effect.Bleed:
                    if (!isSuperEffect)
                    {
                        enemy.effectVFXGameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    }
                    else
                    {
                        //Missing
                    }
                    break;
                case Effect.Chill:
                    if (!isSuperEffect)
                    {
                        if (value)
                        {
                            enemy.effectVFXGameObject.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            enemy.effectVFXGameObject.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        }
                    }
                    else
                    {
                        enemy.effectVFXGameObject.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                    }
                    break;
                case Effect.Wealth:
                    if (!isSuperEffect)
                    {
                        if (value)
                        {
                            enemy.effectVFXGameObject.transform.GetChild(3).GetComponent<ParticleSystem>().Play();
                        }
                        else
                        {
                            enemy.effectVFXGameObject.transform.GetChild(3).GetComponent<ParticleSystem>().Stop();
                        }
                    }
                    else
                    {
                        //Missing
                    }
                    break;
            }
        }

        #endregion
    }
}
