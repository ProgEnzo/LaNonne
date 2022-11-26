using System;
using System.Collections;
using System.Collections.Generic;
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
        
        [SerializeField] internal int effectMaxLevel;
        [SerializeField] internal int numberOfEffects;
        [SerializeField] internal List<ListOfShopSo> effectDictionary = new();

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
        }
    }
}
