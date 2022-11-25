using System;
using System.Collections;
using System.Collections.Generic;
using Shop;
using UnityEngine;

namespace Shop
{
    public class EffectManager : MonoBehaviour
    {
        internal enum Effect
        {
            Bleed = 0,
            Chill = 1,
            Target = 2,
            Wealth = 3
        }

        private Dictionary<Effect, List<ShopObjectSO>> effectDictionary = new();

        void Start()
        {
        
        }

        void Update()
        {
        
        }
    }
}
