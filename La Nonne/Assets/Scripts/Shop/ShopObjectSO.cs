using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopObject", menuName = "ShopObject")]
public class ShopObjectSO : ScriptableObject
{
   public string effectName = "weapon Name Here";
   public int level = 1;
   public int cost = 50;
   public string description;

   public float rateOfDamage = 0.5f;
   public int damage = 10;
   public int radius = 25;
   public float coolDown = 2f;
   public int epDropRate = 5;
   public float movementSpeed = 10f;
}
