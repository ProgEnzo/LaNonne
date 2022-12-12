using UnityEngine;

namespace AI.So
{
    public class SO_Enemy : ScriptableObject
    {
        [Header("Enemy Health")] 
        public float maxHealth;
    
        [Header("Attack Values")] 
        public int bodyDamage;
        
        [Header("Movement Values")]
        public float aiPathBasicSpeed;
        public float velocityBasicSpeed;
        
        [Header("EP Values")]
        public int numberOfEp;

        [Header("Score Values")] 
        public int scorePoint;
    }
}
