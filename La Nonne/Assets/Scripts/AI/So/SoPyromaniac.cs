using UnityEngine;

namespace AI.So
{
    [CreateAssetMenu(fileName = "Pyromaniac", menuName = "ScriptableObjects/Pyromaniac", order = 1)]
    public class SoPyromaniac : SO_Enemy
    {
        [Header("Detection Values")]
        [SerializeField] public float detectionRadius;
        
        [Header("Throw Values")]
        [SerializeField] public float throwRadius;
        [SerializeField] public float projectileSpeed;
        
        [Header("Fire Trail Values")]
        [SerializeField] public float fireTrailTolerance;
        [SerializeField] public float fireTrailDisappearanceSpeed;
        
        [Header("Fire Values")]
        [SerializeField] public float explosionRadius;
        [SerializeField] public float fireCooldown;
        
        [Header("Projectile Values")]
        [SerializeField] public float explosionRadiusIndicator;
        [SerializeField] public float knockBackPower;
    }
}
