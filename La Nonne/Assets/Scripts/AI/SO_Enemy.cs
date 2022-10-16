using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class SO_Enemy : ScriptableObject
{
    [Header("Enemy Health")] 
    public float maxHealth;
    
    [Header("Attack Values")] 
    public float attackDamage;
    
   
}
