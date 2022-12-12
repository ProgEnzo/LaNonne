using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
   public class ShopObjectSO : ScriptableObject
   {
      [Header("General")] 
      //[SerializeField] internal Sprite image;
      [SerializeField] internal int level = 1;
      [SerializeField] internal int cost = 50;
      [SerializeField] internal string description;
      [SerializeField] internal float chanceToBeApplied = 10;
   }
}
