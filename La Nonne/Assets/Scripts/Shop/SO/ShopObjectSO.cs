using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Shop
{
   public class ShopObjectSO : ScriptableObject
   {
      [Header("General")] 
      [SerializeField] internal Sprite image;
      [SerializeField] internal int level = 1;
      [SerializeField] internal int cost = 50;
      [SerializeField] internal float chanceToBeApplied = 10;
      [SerializeField] internal string shopDescription;
      [SerializeField] internal string effectDescription;
      [SerializeField] internal string superEffectDescription;
   }
}
